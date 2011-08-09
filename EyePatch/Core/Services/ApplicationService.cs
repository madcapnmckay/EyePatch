using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Linq;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Plugins;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Exceptions;
using EyePatch.Core.Widgets;
using EyePatch.Core.Entity;
using StructureMap;

namespace EyePatch.Core.Services
{
    public class ApplicationService : ServiceBase, IApplicationService
    {
        protected IPageService pageService;
        protected ITemplateService templateService;
        protected IFolderService folderService;
        protected IPluginService pluginService;
        protected IMembershipService membershipService;
        protected IFormsAuthenticationService formsService;
        
        public ApplicationService(EyePatchDataContext context, IPageService pageService, ITemplateService templateService, IFolderService folderService, 
            IPluginService pluginService, IMembershipService membershipService, IFormsAuthenticationService formsService)
            : base(context)
        {
            this.pageService = pageService;
            this.pluginService = pluginService;
            this.templateService = templateService;
            this.folderService = folderService;
            this.membershipService = membershipService;
            this.formsService = formsService;
        }

        protected void CreateDefaultPage(string siteName)
        {
            using (var tx = new TransactionScope())
            {
                pageService.Create("Home", "Home", "/", true);

                tx.Complete();
            }
        }

        public void Install(InstallationForm install)
        {
            var createStatus = membershipService.CreateUser(install.Username, install.Password, install.Email);

            if (createStatus != MembershipCreateStatus.Success)
                throw new InstallationException("Member creation failed", 2, createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                membershipService.CreateRole("Admin");
                membershipService.AddUserToRole("Admin", install.Username);
                formsService.SignIn(install.Username, false);

                // create site
                db.Sites.InsertOnSubmit(new Site { Email = install.Email, Name = install.SiteName });
                db.SubmitChanges();

                // rename root to sitename
                folderService.RootFolder.Name = install.SiteName;
                db.SubmitChanges();

                CreateDefaultPage(install.SiteName);
            }
        }

        public bool SignIn(string userName, string password)
        {
            if (membershipService.ValidateUser(userName, password))
            {
                formsService.SignIn(userName, false);
                return true;
            }
            throw new ApplicationException("Invalid username or password");
        }

        public void SignOut()
        {
            // any other tasks we need to perform
            formsService.SignOut();
        }

        public void Initialise()
        {
            using (var conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["EyePatchDb"].ConnectionString))
            {
                conn.Open();

                if (!DataTablesExist(conn))
                    DataSourceTools.CreateDatabaseTables(conn, new[] {
                                     "InstallCommon.sql",
                                     "InstallRoles.sql",
                                     "InstallMembership.sql",
                                     "InstallPersonalization.sql",
                                     "InstallProfile.sql",
                                     "InstallWebEventSqlProvider.sql",
                                     "InstallEyepatch.sql"
                                 });
            }

            // Check for templates
            var path = HttpContext.Current.Server.MapPath(EyePatchConfig.TemplateDir);
            var di = new DirectoryInfo(path);
            var files = di.GetFiles("*.cshtml").Where(f => !Path.GetFileNameWithoutExtension(f.FullName).StartsWith("_"));

            if (files.Count() == 0)
                throw new InstallationException("There must be at least one template file in the Templates directory", 1);

            templateService.CreateTemplates(files.Select(f => f.FullName).ToList());
            if (templateService.DefaultTemplate == null && db.Templates.Any())
            {
                // there must be a default template, set to default.cshtml
                var defaultTemplate = db.Templates.SingleOrDefault(
                    t => t.ViewPath.EndsWith("default.cshtml")) ?? db.Templates.First();

                defaultTemplate.IsDefault = true;
                db.SubmitChanges();
            }

            // create the root folder, so that plugins creating pages can use it
            if (folderService.RootFolder == null)
                folderService.Create("Root", -1);

            // call startup on all plugins
            foreach (var plugin in ObjectFactory.Model.GetAllPossible<IEyePatchPlugin>())
            {
                pluginService.Register(plugin);
                plugin.Startup();
            }
        }

        protected static bool DataTablesExist(SqlConnection connection)
        {
            var restrictions = new string[4];
            restrictions[3] = "Page";
            var table = connection.GetSchema("Tables", restrictions);
            return table.Rows.Count > 0;
        }

        
    }
}