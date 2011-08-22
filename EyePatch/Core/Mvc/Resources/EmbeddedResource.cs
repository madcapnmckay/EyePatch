using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EyePatch.Core.Mvc.Resources
{
    public class EmbeddedResource : Resource
    {
        protected Assembly assembly;

        public EmbeddedResource(string path, string rootNamespace, Assembly assembly)
            : base(string.Format("{0}.{1}", rootNamespace, path.Trim().Trim('/').Replace("/", ".")), MatchMode.FileName)
        {
            this.assembly = assembly;
        }

        public override FileInfo DependentFile
        {
            get { return new FileInfo(assembly.Location); }
        }

        public override string FileContents()
        {
            var allresources = assembly.GetManifestResourceNames();
            var resourceName =
                assembly.GetManifestResourceNames().SingleOrDefault(r => string.Compare(Url, r, true) == 0);
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ApplicationException("The resource cannot be found");

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ApplicationException("The resource cannot be read");

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}