using System;
using System.Collections.Generic;
using System.Linq;
using EyePatch.Core.Entity;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Plugins;
using EyePatch.Core.Widgets;
using NKnockoutUI.Window;
using StructureMap;

namespace EyePatch.Core.Services
{
    public class PluginService : ServiceBase, IPluginService
    {
        protected IWidgetService widgetService;

        public PluginService(EyePatchDataContext context, IWidgetService widgetService)
            : base(context)
        {
            this.widgetService = widgetService;
        }

        public IList<Plugin> All()
        {
            return db.Plugins.ToList();
        }

        public Plugin Load(Type type)
        {
            return db.Plugins.SingleOrDefault(p => p.FullName == type.AssemblyQualifiedName);
        }

        public Plugin Create(IEyePatchPlugin plugin)
        {
            if (string.IsNullOrWhiteSpace(plugin.Name))
                throw new ApplicationException("The plugin must have a non-empty name");

            var newPlugin = new Plugin
                                {
                                    Name = plugin.Name,
                                    FullName = plugin.GetType().AssemblyQualifiedName,
                                    Author = plugin.Author ?? "Unknown"
                                };

            
            db.Plugins.InsertOnSubmit(newPlugin);
            db.SubmitChanges();
            return newPlugin;
        }

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

        public bool Register(IEyePatchPlugin pluginDef)
        {
            var plugin = Load(pluginDef.GetType());
            if (plugin == null)
            {
                plugin = Create(pluginDef);

                pluginDef.Register();

                foreach (var widgetDef in pluginDef.Widgets.Select(ObjectFactory.GetInstance).OfType<IWidget>())
                {
                    if (string.IsNullOrWhiteSpace(widgetDef.Name))
                        throw new ApplicationException("The widget must have a non-empty name");

                    var widget = widgetService.Load(widgetDef.GetType());
                    if (widget == null)
                    {

                        widget = new Widget
                                            {Name = widgetDef.Name, Type = widgetDef.GetType().AssemblyQualifiedName, Plugin = plugin };
                        db.Widgets.InsertOnSubmit(widget);
                        db.SubmitChanges();
                    }
                }
                return true;
            }
            return false;
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
    }
}