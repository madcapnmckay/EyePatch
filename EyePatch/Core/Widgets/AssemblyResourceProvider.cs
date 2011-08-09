using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace EyePatch.Core.Widgets
{
    public class AssemblyResourceProvider : VirtualPathProvider
    {
        private bool IsAppResourcePath(string virtualPath)
        {
            var checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            return checkPath.StartsWith(ContentManager.PluginDir, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool FileExists(string virtualPath)
        {
            return (IsAppResourcePath(virtualPath) || base.FileExists(virtualPath));
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return IsAppResourcePath(virtualPath) ? new AssemblyResourceVirtualFile(virtualPath) : base.GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies,
                                                           DateTime utcStart)
        {
            return IsAppResourcePath(virtualPath) ? null : base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
    }

    internal class AssemblyResourceVirtualFile : VirtualFile
    {
        private readonly string path;

        public AssemblyResourceVirtualFile(string virtualPath)
            : base(virtualPath)
        {
            path = VirtualPathUtility.ToAppRelative(virtualPath);
        }

        public override Stream Open()
        {
            var parts = path.Split('/');
            var resourceName = Path.GetFileName(path);

            var apath = HttpContext.Current.Server.MapPath(Path.GetDirectoryName(path));
            try
            {
                var assembly = Assembly.LoadFile(apath);
                var names = assembly.GetManifestResourceNames();

                if (names.Count() > 0)
                {
                    var matched = names.SingleOrDefault(s => string.Compare(s, resourceName, true) == 0);

                    if (matched != null)
                        return assembly.GetManifestResourceStream(matched);
                }
                return null;
            } 
            catch
            {
                return null;
            }
        }
    }
}