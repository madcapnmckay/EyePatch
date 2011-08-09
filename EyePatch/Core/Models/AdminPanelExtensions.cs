using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Models.Tree;
using EyePatch.Core.Models.Tree.Nodes;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Util.Extensions;
using EyePatch.Core.Widgets;
using Newtonsoft.Json.Serialization;

namespace EyePatch.Core.Models
{
    public static class AdminPanelExtensions
    {
        public static PageTree ToViewModel(this IEnumerable<Entity.Folder> folders)
        {
            var result = new PageTree();
            var hierarchy = folders.AsHierarchy(k => k.ID, p => p.ParentID).ToList();
            hierarchy.ForEach(result.AddFolder);
            return result;
        }

        public static WidgetTree ToViewModel(this IEnumerable<Entity.Plugin> plugins)
        {
            var result = new WidgetTree();
            var hierarchy = plugins.ToList();
            hierarchy.ForEach(result.AddWidgetGroup);
            return result;
        }

        public static TemplateTree ToViewModel(this IEnumerable<Entity.Template> templates)
        {
            var result = new TemplateTree();
            var hierarchy = templates.ToList();
            hierarchy.ForEach(result.AddTemplate);
            return result;
        }

        public static MediaTree ToViewModel(this DirectoryInfo root)
        {
            var result = new MediaTree();
            result.Children.Add(new MediaFolderNode(root));
            return result;
        }

        public static PageForm ToViewModel(this Entity.Page page, Entity.Page homepage)
        {
            var result = new PageForm();
            result.Id = page.ID;
            result.IsLive = page.IsLive;
            result.TemplateID = page.TemplateID;
            result.Title = page.Title ?? string.Empty;
            result.Url = page.Url ?? string.Empty;
            result.UrlEditable = page.ID != homepage.ID;
            result.IsInMenu = page.IsInMenu;
            result.MenuOrder = page.MenuOrder;
            return result;
        }

        public static PageSearchForm ToSearchViewModel(this Entity.Page page)
        {
            var form = new PageSearchForm();
            form.Id = page.ID;
            form.Description = page.Description ?? string.Empty;
            form.Keywords = page.Keywords ?? string.Empty;
            form.Language = page.Language ?? 1033;
            form.Charset = page.Charset ?? "UTF-8";
            form.Author = page.Author ?? string.Empty;
            form.Copyright = page.Copyright ?? string.Empty;
            form.Robots = page.Robots ?? string.Empty;

            form.Def = page.Template.ToSearchViewModel();
            return form;
        }

        public static PageFacebookForm ToFacebookViewModel(this Entity.Page page)
        {
            var form = new PageFacebookForm();
            form.Id = page.ID;
            form.Type = page.OgType ?? string.Empty;
            form.Email = page.OgEmail ?? string.Empty;
            form.Phone = page.OgPhone ?? string.Empty;
            form.Image = page.OgImage ?? string.Empty;
            form.StreetAddress = page.OgStreetAddress ?? string.Empty;
            form.Locality = page.OgLocality ?? string.Empty;
            form.Region = page.OgRegion ?? string.Empty;
            form.Country = page.OgCountry ?? string.Empty;
            form.Postcode = page.OgPostcode ?? string.Empty;
            form.Longitude = page.OgLongitude;
            form.Latitude = page.OgLatitude;

            form.Def = page.Template.ToFacebookViewModel();
            return form;
        }

        public static TemplateForm ToViewModel(this Entity.Template template)
        {
            return new TemplateForm { Id = template.ID, AnalyticsKey = template.AnalyticsKey };
        }

        public static SearchForm ToSearchViewModel(this Entity.Template template)
        {
            var form = new SearchForm();
            form.Id = template.ID;
            form.Description = template.Description ?? string.Empty;
            form.Keywords = template.Keywords ?? string.Empty;
            form.Language = template.Language ?? 1033;
            form.Charset = template.Charset ?? "UTF-8";
            form.Author = template.Author ?? string.Empty;
            form.Copyright = template.Copyright ?? string.Empty;
            form.Robots = template.Robots ?? string.Empty;
            return form;
        }

        public static FacebookForm ToFacebookViewModel(this Entity.Template template)
        {
            var form = new FacebookForm();
            form.Id = template.ID;
            form.Type = template.OgType ?? string.Empty;
            form.Email = template.OgEmail ?? string.Empty;
            form.Phone = template.OgPhone ?? string.Empty;
            form.Image = template.OgImage ?? string.Empty;
            form.StreetAddress = template.OgStreetAddress ?? string.Empty;
            form.Locality = template.OgLocality ?? string.Empty;
            form.Region = template.OgRegion ?? string.Empty;
            form.Country = template.OgCountry ?? string.Empty;
            form.Postcode = template.OgPostcode ?? string.Empty;
            form.Longitude = template.OgLongitude;
            form.Latitude = template.OgLatitude;
            return form;
        }

        public static WidgetInstance ToViewModel(this Entity.WidgetInstance pageWidget, IContentManager contentManager, Controller controller)
        {
            var widget = pageWidget.GetInstance();

            var pageJs = contentManager.Page.Js(pageWidget.ContentArea.PageID);
            var pageCss = contentManager.Page.Css(pageWidget.ContentArea.PageID);

            pageJs.AddRange(AdminPanelViewModel.DependentJs);
            pageCss.AddRange(AdminPanelViewModel.DependentCss);

            var remainingJs =
                contentManager.Resources.GetResourcePaths(widget.AdminJs.Except(pageJs).ToResourceCollection()).ToList();

            var remainingCss =
                contentManager.Resources.GetResourcePaths(widget.AdminCss.Except(pageCss).ToResourceCollection()).ToList();

            string contents;
            using (var sw = new MemoryStream())
            {
                using (var writer = new HtmlTextWriter(new StringWriter()))
                {
                    widget.Render(new WidgetContext(controller.ControllerContext, pageWidget, writer));
                    contents = writer.InnerWriter.ToString();
                }
            }

            return new WidgetInstance
                             {
                                 Id = pageWidget.ID,
                                 InitializeFunction = widget.CreateFunction,
                                 CssClass = widget.CssClass,
                                 Js = remainingJs,
                                 Css = remainingCss,
                                 Contents = contents
                             };

        }
    }
}