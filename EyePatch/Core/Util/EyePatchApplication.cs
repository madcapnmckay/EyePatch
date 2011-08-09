using System.Web;
using EyePatch.Core.IoC;
using StructureMap;

namespace EyePatch.Core.Util
{
    /// <summary>
    /// Startup bootstrapper that performs tasks on app start
    /// </summary>
    public static class EyePatchApplication
    {
        public static string SiteID
        {
            get { return HttpContext.Current.Request.Url.Authority; }
        }

        public static void Startup()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new CoreRegistry());
                x.AddRegistry(new PluginRegistry());
            });

            // initialise the content manager
            ObjectFactory.GetInstance<IContentManager>().Application.Initialise();
        }

        public static ReleaseMode ReleaseMode
        {
            get
            {
#if DEBUG
                return Util.ReleaseMode.Debug;
#elif STAGING
                return Util.ReleaseMode.Staging;
#else
                return Util.ReleaseMode.Production;
#endif

            }
        }
    }
}