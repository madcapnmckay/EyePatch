using System.Collections.Generic;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Plugins;
using NKnockoutUI.Window;

namespace EyePatch.Core.Services
{
    public interface IPluginService
    {
        IList<Window> UserInterfaceWindows();
        ResourceCollection UserInterfaceJs();
        ResourceCollection UserInterfaceCss();

        /// <summary>
        ///   Iterates over all known plugins and interogates them to provide the items
        ///   they wish to be included in the sitemap.xml
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISiteMapItem> SiteMapItems();

        IEnumerable<IEyePatchPlugin> All();
    }
}