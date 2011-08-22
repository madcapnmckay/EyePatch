using System.Collections.Generic;
using System.Linq;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Plugins;
using NKnockoutUI.Window;
using StructureMap;

namespace EyePatch.Core.Services
{
    public class PluginService : IPluginService
    {
        protected IWidgetService widgetService;

        public PluginService(IWidgetService widgetService)
        {
            this.widgetService = widgetService;
        }

        #region IPluginService Members

        public IList<Window> UserInterfaceWindows()
        {
            var windows = new List<Window>();
            foreach (var plugin in ObjectFactory.Model.GetAllPossible<IEyePatchPlugin>())
            {
                windows.AddRange(plugin.Windows);
            }
            return windows;
        }

        public ResourceCollection UserInterfaceJs()
        {
            var js = new ResourceCollection();
            foreach (var plugin in ObjectFactory.Model.GetAllPossible<IEyePatchPlugin>())
            {
                js.AddRange(plugin.Js);
            }
            return js;
        }

        public ResourceCollection UserInterfaceCss()
        {
            var css = new ResourceCollection();
            foreach (var plugin in ObjectFactory.Model.GetAllPossible<IEyePatchPlugin>())
            {
                css.AddRange(plugin.Css);
            }
            return css;
        }

        public IEnumerable<ISiteMapItem> SiteMapItems()
        {
            var nodes = new List<ISiteMapItem>();
            foreach (var plugin in ObjectFactory.Model.GetAllPossible<IEyePatchPlugin>())
            {
                nodes.AddRange(plugin.SiteMapItems() ?? Enumerable.Empty<ISiteMapItem>());
            }
            return nodes;
        }

        public IEnumerable<IEyePatchPlugin> All()
        {
            return ObjectFactory.GetAllInstances<IEyePatchPlugin>();
        }

        #endregion
    }
}