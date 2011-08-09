using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace EyePatch.Core.Util
{
    public static class DataSourceTools
    {
        private static string connString;
        public static string DbConnectionString
        {
            get
            {
                return connString ??
                       (connString = WebConfigurationManager.ConnectionStrings["EyePatchDb"].ConnectionString);
            }
        }

        public static bool CreateDatabaseTables(string sql)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                return CreateDatabaseTables(conn, sql);
            }
        }

        public static bool CreateDatabaseTables(string[] fileNames)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                return CreateDatabaseTables(conn, fileNames);
            }
        }

        public static bool CreateDatabaseTables(SqlConnection connection, string sql)
        {
            // Declare list of .sql files, 
            // notice that InstallCommon.sql must be installed first
            // adapted from http://www.beansoftware.com/ASP.NET-Tutorials/SQL-Membership-Schema-Run-Time.aspx
            var comm = new SqlCommand();

            comm.Connection = connection;
            comm.CommandType = CommandType.Text;

            // replace default database name 'aspnetdb' to name of your database
            sql = sql.Replace("aspnetdb", connection.Database);

            try
            {
                ExecuteSql(comm, sql);
            }
            catch
            {
                return false;
            }
            comm.Dispose();
            return true;
        }

        public static bool CreateDatabaseTables(SqlConnection connection, string[] fileNames)
        {
            // Declare list of .sql files, 
            // notice that InstallCommon.sql must be installed first
            // adapted from http://www.beansoftware.com/ASP.NET-Tutorials/SQL-Membership-Schema-Run-Time.aspx
            var comm = new SqlCommand();

            comm.Connection = connection;
            comm.CommandType = CommandType.Text;

            // Iterates through all files
            foreach (var t in fileNames)
            {
                // Open current SQL script from SQL/ sub folder
                var fileContent = File.ReadAllText(HttpContext.Current.Server.MapPath("/Core/SQL/") + t, Encoding.UTF8);

                // replace default database name 'aspnetdb' to name of your database
                fileContent = fileContent.Replace("aspnetdb", connection.Database);

                try
                {
                    ExecuteSql(comm, fileContent);
                }
                catch
                {
                    return false;
                }
            }
            comm.Dispose();
            return true;
        }

        public static bool TableExists(string tableName)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                return TableExists(conn, tableName);
            }
        }

        public static bool TableExists(SqlConnection connection, string tableName)
        {
             var restrictions = new string[4];
                restrictions[2] = "Blog";
                var table = connection.GetSchema("Tables", restrictions);

            return table.Rows.Count > 0;
        }

        public static void ExecuteSql(SqlCommand command, string sql)
        {
            // Loads string to StreamReader
            string currentLine;
            var sqlQuery = "";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(sql));
            var sr = new StreamReader(ms);

            while (!sr.EndOfStream)
            {
                currentLine = sr.ReadLine();
                // Check if line is empty
                if (!string.IsNullOrEmpty(currentLine))
                {
                    if (currentLine.Trim().ToUpper() != "GO")
                    {
                        // Build Sql to execute
                        sqlQuery += currentLine + "\n";
                    }
                    else
                    {
                        // Current line is 'GO' so execute code chunk
                        command.CommandText = sqlQuery;
                        command.ExecuteNonQuery();
                        sqlQuery = "";
                    }
                }
            }
        }
    }
}