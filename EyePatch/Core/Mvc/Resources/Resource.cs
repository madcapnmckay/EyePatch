using System;
using System.IO;
using System.Text.RegularExpressions;

namespace EyePatch.Core.Mvc.Resources
{
    public class Resource : IEquatable<Resource>
    {
        protected static Regex externalRegex = new Regex("^(http(s)?:)?//", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        protected string contentType;

        public Resource(string path, IResourcePathProvider pathProvider, MatchMode matchMode)
        {
            if (pathProvider == null) throw new ArgumentNullException("pathProvider");
            if (string.IsNullOrWhiteSpace(path)) throw new ApplicationException("Resource path cannot be empty");

            path = path.Trim();
            Url = path;
            MatchMode = matchMode;

            if (!IsExternal)
            {
                PhysicalFilePath = pathProvider.MapPath(path);
            }
        }

        public Resource(string path, MatchMode matchMode) : this(path, new ResourcePathProvider(), matchMode) { }
        public Resource(string path, MatchMode matchMode, string contentType)
            : this(path, new ResourcePathProvider(), matchMode)
        {
            this.contentType = contentType;
        }

        public virtual string Url { get; set; }
        public virtual string PhysicalFilePath { get; set; }

        public string FileName { get { return Path.GetFileName(PhysicalFilePath); } }

        public virtual FileInfo DependentFile
        {
            get
            {
                return new FileInfo(PhysicalFilePath);
            }
        }

        public string ContentType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(contentType))
                    return contentType;

                return Url.EndsWith(".js") ? "text/javascript" : "text/css";
            }
        }

        public bool IsExternal
        {
            get { return externalRegex.IsMatch(Url); }
        }

        public string Extension
        {
            get { return Path.GetExtension(PhysicalFilePath); }
        }

        public virtual string FileContents()
        {
            var fileInfo = new FileInfo(PhysicalFilePath);
            using (var reader = fileInfo.OpenText()) { 
                return reader.ReadToEnd(); 
            } 
        }

        public MatchMode MatchMode { get; protected set; }

        public static string Normalize(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            return externalRegex.Replace(path, string.Empty);
        }

        public bool Equals(Resource other)
        {
            //Check whether the compared object is null.
            if (ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other)) return true;

            return string.Compare(Normalize(Url), Normalize(other.Url), true) == 0;
        }

        public override int GetHashCode()
        {
            return Normalize(Url).GetHashCode();
        }
    }
}