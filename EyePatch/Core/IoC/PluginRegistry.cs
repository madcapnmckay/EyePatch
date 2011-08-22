using System.Web;
using System.Web.Mvc;
using EyePatch.Core.Mvc.Routing;
using EyePatch.Core.Plugins;
using EyePatch.Core.Widgets;
using StructureMap.Configuration.DSL;

namespace EyePatch.Core.IoC
{
    public class PluginRegistry : Registry
    {
        public PluginRegistry()
        {
            var widgetDir = HttpContext.Current.Server.MapPath(ContentManager.PluginDir);

            Scan(x =>
                     {
                         x.TheCallingAssembly();
                         x.AddAllTypesOf<IWidget>();
                     });
            Scan(x =>
                     {
                         x.TheCallingAssembly();
                         x.AssembliesFromPath(widgetDir);
                         x.LookForRegistries();
                         x.AddAllTypesOf<IEyePatchPlugin>();
                         x.AddAllTypesOf<IWidget>();
                         x.AddAllTypesOf<IController>();
                         x.AddAllTypesOf<IRouteRegistry>();
                     });
        }
    }
}