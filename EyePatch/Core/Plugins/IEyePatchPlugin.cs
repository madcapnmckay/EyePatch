using System;
using System.Collections.Generic;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Widgets;
using NKnockoutUI.Window;

namespace EyePatch.Core.Plugins
{
    /// <summary>
    /// Eye aye captain! This is the interface that should be implemented in your dll to register a distinct
    /// plugin within EyePatch
    /// </summary>
    public interface IEyePatchPlugin
    {
        /// <summary>
        /// The name of the plugin as it will appear in the widget tab
        /// </summary>
        string Name { get; }
        /// <summary>
        /// A list of widget types associated with this plugin
        /// </summary>
        IList<Type> Widgets { get; }
        /// <summary>
        /// A list of windows the plugin wishes to display
        /// </summary>
        IList<Window> Windows { get; }
        /// <summary>
        /// Js files needed to operate the UI
        /// </summary>
        ResourceCollection Js { get; }
        /// <summary>
        /// Css files needed to style the UI
        /// </summary>
        ResourceCollection Css { get; }
        /// <summary>
        /// The owner of the plugin
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Called when the plugin is first encounted, used to create db etc
        /// </summary>
        void Register();

        /// <summary>
        /// Called on application start
        /// </summary>
        void Startup();

        /// <summary>
        /// This method allows the plugin to optionally return a collection of
        /// ISiteMapItems which will be included in the sitemap.xml
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISiteMapItem> SiteMapItems();
    }
}
