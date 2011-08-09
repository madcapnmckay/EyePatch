using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using EyePatch.Core.Entity;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Plugins;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;
using Microsoft.Ajax.Utilities;
using StructureMap;

namespace EyePatch.Core.Services
{
    public class ResourceService : ServiceBase, IResourceService
    {
        protected ICacheProvider cacheProvider;
        protected string resourceBaseUrl = WebConfigurationManager.AppSettings["ResourceCache"];
        private static ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private readonly CodeSettings jsSettings;
        private readonly CssSettings cssSettings;

        public ResourceService(EyePatchDataContext context, ICacheProvider cacheProvider)
            : base(context)
        {
            this.cacheProvider = cacheProvider;

            jsSettings = new CodeSettings
                    {
                        MinifyCode = true,
                        OutputMode = OutputMode.SingleLine,
                        CollapseToLiteral = true,
                        CombineDuplicateLiterals = true,
                        EvalTreatment = EvalTreatment.Ignore,
                        InlineSafeStrings = true,
                        LocalRenaming = LocalRenaming.CrunchAll,
                        MacSafariQuirks = true,
                        PreserveFunctionNames = false,
                        RemoveFunctionExpressionNames = true,
                        RemoveUnneededCode = true,
                        StripDebugStatements = true,
                    };

            cssSettings = new CssSettings
            {
                AllowEmbeddedAspNetBlocks = false,
                CommentMode = CssComment.Hacks,
                ExpandOutput = false,
                TermSemicolons = true,
                MinifyExpressions = true,
                ColorNames = CssColor.Hex
            };
        }

        public IEnumerable<ResourcePath> GetResourcePaths(ResourceCollection resources)
        {
            return GetResourcePaths(resources, 
                EyePatchApplication.ReleaseMode == ReleaseMode.Production, 
                EyePatchApplication.ReleaseMode == ReleaseMode.Production, 
                EyePatchApplication.ReleaseMode == ReleaseMode.Production);
        }

        public IEnumerable<ResourcePath> GetResourcePaths(ResourceCollection resources, bool mash, bool minify, bool cache)
        {
            if (!mash)
                return resources.Select(r => ProcessResource(r, minify, cache));

            var groups = GroupResources(resources);

            return groups.Select(set => ProcessGroup(set, minify, cache)).ToList();
        }

        public ResourcePath GetContentsFor(string identifer)
        {
            var cachedVersion = cacheProvider.Get<ResourcePath>(identifer);
            if (cachedVersion != null)
                return cachedVersion;

            // look for the physical file
            // backup step only
            var physicalFilePath = GetFilePath(identifer);
            if (File.Exists(physicalFilePath))
            {
                var result = new ResourcePath();
                result.Contents = File.ReadAllText(physicalFilePath);
                result.FileName = physicalFilePath;
                result.Url = Url.Action("fetch", "resource", new { id = identifer });
            }
            return null;
        }

        public byte[] EmbeddedResource(string resourceName)
        {
            var assemblies = new List<Assembly>();
            assemblies.AddRange(ObjectFactory.GetAllInstances<IEyePatchPlugin>().Select(t => t.GetType().Assembly).Distinct());

            var resourceAssembly = assemblies.Where(a => a.GetManifestResourceNames().Any(r => string.Compare(r, resourceName, true, CultureInfo.InvariantCulture) == 0)).First();

            if (resourceAssembly == null)
                throw new ApplicationException("The resource cannot be found");

            using (var stream = resourceAssembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ApplicationException("The resource cannot be read");

                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
        }

        private ResourcePath ProcessGroup(ResourceCollection set, bool minify, bool cacheRefresh)
        {
            if (!set.Any())
                throw new ApplicationException("Cannot process empty group");

            var firstElement = set.First();
            if (set.Count() == 1 && firstElement.IsExternal)
                return new ResourcePath { ContentType = firstElement.ContentType, Url = firstElement.Url };

            var cacheKey = minify ? set.UnqiueId.ToMinPath() : set.UnqiueId;

            var upgraded = false;
            try
            {
                cacheLock.EnterUpgradeableReadLock();

                var cached = cacheProvider.Get<ResourcePath>(cacheKey);
                if (cached != null && cacheRefresh)
                    return cached;

                cacheLock.EnterWriteLock();
                upgraded = true;

                var priorWrite = cacheProvider.Get<ResourcePath>(cacheKey);
                if (priorWrite != null && cacheRefresh)
                    return priorWrite;

                // regenerate
                var result = new ResourcePath();
                result.ContentType = firstElement.ContentType;
                result.Url = Url.Action("fetch", "resource", new { id = cacheKey });

                // mash
                result.Contents = set.Mash();

                // minify
                if (minify)
                {
                    var minifier = new Minifier();
                    result.Contents = firstElement.ContentType == "text/javascript"
                                          ? minifier.MinifyJavaScript(result.Contents, jsSettings)
                                          : minifier.MinifyStyleSheet(result.Contents, cssSettings);
                }

                // write backup file
                var physicalFilePath = GetFilePath(set.UnqiueId, minify);
                if (File.Exists(physicalFilePath))
                    File.Delete(physicalFilePath);

                File.WriteAllText(physicalFilePath, result.Contents);
                result.FileName = physicalFilePath;

                cacheProvider.Add(cacheKey, result, set.Files());
                return result;
            }
            finally
            {
                if (upgraded)
                    cacheLock.ExitWriteLock();

                cacheLock.ExitUpgradeableReadLock();
            }
        }

        private ResourcePath ProcessResource(Resource resource, bool minify, bool cacheRefresh)
        {
            if (resource == null) throw new ArgumentNullException("resource");

            if (!(resource is EmbeddedResource) && (resource.IsExternal || !minify))
                return new ResourcePath { ContentType = resource.ContentType, Url = resource.Url };

            var cacheKey = resource.FileName.ToMinPath();

            var upgraded = false;
            try
            {
                cacheLock.EnterUpgradeableReadLock();

                var cached = cacheProvider.Get<ResourcePath>(cacheKey);
                if (cached != null && cacheRefresh)
                    return cached;

                cacheLock.EnterWriteLock();
                upgraded = true;

                var priorWrite = cacheProvider.Get<ResourcePath>(cacheKey);
                if (priorWrite != null && cacheRefresh)
                    return priorWrite;

                // regenerate
                var result = new ResourcePath();
                result.ContentType = resource.ContentType;
                result.Contents = resource.FileContents();
                result.Url = Url.Action("fetch", "resource", new { id = cacheKey });

                // minify

                var minifier = new Minifier();
                if (minify)
                {
                    result.Contents = resource.ContentType == "text/javascript"
                                          ? minifier.MinifyJavaScript(result.Contents, jsSettings)
                                          : minifier.MinifyStyleSheet(result.Contents, cssSettings);
                } 

                // write backup file
                var physicalFilePath = GetFilePath(cacheKey, false);
                if (File.Exists(physicalFilePath))
                    File.Delete(physicalFilePath);

                File.WriteAllText(physicalFilePath, result.Contents);
                result.FileName = physicalFilePath;

                cacheProvider.Add(cacheKey, result, resource.DependentFile);
                return result;
            }
            finally
            {
                if (upgraded)
                    cacheLock.ExitWriteLock();

                cacheLock.ExitUpgradeableReadLock();
            }
        }

        private string GetFilePath(string unqiueId)
        {
            return HttpContext.Current.Server.MapPath(Util.Url.Combine(resourceBaseUrl, unqiueId));
        }

        private string GetFilePath(string unqiueId, bool minify)
        {
            return minify ? GetFilePath(unqiueId).ToMinPath() : GetFilePath(unqiueId);
        }

        private static IEnumerable<ResourceCollection> GroupResources(IEnumerable<Resource> resources)
        {
            var groups = new List<ResourceCollection>();
            foreach (var resource in resources)
            {
                ResourceCollection group = groups.LastOrDefault();
                if (resource.IsExternal || group == null || group.Last().IsExternal)
                {
                    group = new ResourceCollection();
                    groups.Add(group);
                }
                group.Add(resource);
            }
            return groups;
        }

        public string MimeType(string filename)
        {
            var mime = "application/octetstream";
            var ext = Path.GetExtension(filename).ToLower();
            var rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();
            return mime;
        }
    }
}