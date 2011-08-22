using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using EyePatch.Core.IoC;
using EyePatch.Core.Plugins;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;
using Raven.Client.Indexes;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace EyePatch.Core.Util
{
    /// <summary>
    ///   Startup bootstrapper that performs tasks on app start
    /// </summary>
    public static class EyePatchApplication
    {
        public static string SiteID
        {
            get { return HttpContext.Current.Request.Url.Authority; }
        }

        public static ReleaseMode ReleaseMode
        {
            get
            {
#if DEBUG
                return ReleaseMode.Debug;
#elif STAGING
                return Util.ReleaseMode.Staging;
#else
                return Util.ReleaseMode.Production;
#endif
            }
        }

        public static bool HasPages { get; set; }

        public static List<Assembly> SystemAssemblies()
        {
            var result = new List<Assembly>();
            result.AddRange(
                ObjectFactory.GetAllInstances<IEyePatchPlugin>().Select(p => p.GetType().Assembly).Distinct());
            return result;
        }

        public static void Startup()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry(new CoreRegistry());
                                             x.AddRegistry(new PluginRegistry());
                                         });

            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));

            var docStore = ObjectFactory.GetInstance<IDocumentStore>();
            SystemAssemblies().ForEach(a => IndexCreation.CreateIndexes(a, docStore));

            // initialise the content manager
            ObjectFactory.GetInstance<IContentManager>().Application.Initialise();
        }
    }
}