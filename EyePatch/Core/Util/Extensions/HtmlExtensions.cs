using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using EyePatch.Core.Documents.Extensions;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Services;
using EyePatch.Core.Widgets;
using StructureMap;

namespace EyePatch.Core.Util.Extensions
{
    public static class HtmlExtensions
    {
        public static void ContentArea<TModel>(this HtmlHelper<TModel> htmlHelper, string areaName)
        {
            var viewBag = htmlHelper.ViewContext.Controller.ViewBag;
            var contentAreas = viewBag.ContentAreas as IList<ContentArea>;
            if (contentAreas == null)
                throw new ApplicationException("Content areas not found");

            // get content area
            var area = contentAreas.SingleOrDefault(c => c.Name == areaName);

            IList<Widget> widgetInstances = null;
            if (area == null)
            {
                var pageService = ObjectFactory.GetInstance<IPageService>();
                area = pageService.CreateContentArea(viewBag.Page, areaName);
                // empty since there can be no widgets
                widgetInstances = new List<Widget>();
            }
            else
            {
                // get widgets for this area
                widgetInstances = area.Widgets.ToList();
            }

            var container = new TagBuilder("div");
            container.GenerateId(areaName);
            container.AddCssClass("content-area");
            container.Attributes["data-name"] = areaName;
            container.Attributes["data-id"] = area.Name;

            var writer = htmlHelper.ViewContext.Writer;
            writer.Write(container.ToString(TagRenderMode.StartTag));

            foreach (var pageWidget in widgetInstances)
            {
                var widget = pageWidget.GetInstance();
                var widgetContainer = new TagBuilder("div");
                var contents = new TagBuilder("div");
                contents.AddCssClass("widget-contents");

                // assign custom type css);
                if (!string.IsNullOrWhiteSpace(widget.CssClass))
                    widgetContainer.AddCssClass(widget.CssClass);

                // assign widget css
                widgetContainer.AddCssClass("ep-widget");

                // assign the id
                widgetContainer.Attributes["data-id"] = pageWidget.Id;
                writer.Write(widgetContainer.ToString(TagRenderMode.StartTag));
                writer.Write(contents.ToString(TagRenderMode.StartTag));

                // render the contents of the widget
                widget.Render(new WidgetContext(htmlHelper.ViewContext, pageWidget, htmlHelper.ViewContext.Writer, htmlHelper.ViewContext.HttpContext.Request.Path));

                writer.Write(contents.ToString(TagRenderMode.EndTag));
                writer.Write(widgetContainer.ToString(TagRenderMode.EndTag));
            }

            writer.Write(container.ToString(TagRenderMode.EndTag));

            // remove from the list so we can detect orphaned content areas later
            contentAreas.Remove(area);
        }

        /// <summary>
        ///   Renders the js for the page
        /// </summary>
        /// <typeparam name = "TModel"></typeparam>
        /// <param name = "htmlHelper"></param>
        /// <param name = "userScriptPaths">Additional scripts supplied by the user</param>
        public static MvcHtmlString RenderJs<TModel>(this HtmlHelper<TModel> htmlHelper, params string[] userScriptPaths)
        {
            var resourceService = ObjectFactory.GetInstance<IResourceService>();
            var builder = new StringBuilder();

            var pageResources = new ResourceCollection();
            // include the widget scripts for the page
            pageResources.AddRange(htmlHelper.ViewContext.Controller.ViewBag.Js);

            // include the template scripts for the page
            foreach (var script in userScriptPaths)
            {
                pageResources.Load(script, MatchMode.Path);
            }
            // we treat these separately because we don't want the admin js to be mashed or cached
            var adminJs = htmlHelper.ViewContext.Controller.ViewBag.AdminJs as ResourceCollection;
            var adminScriptTags = resourceService.GetResourcePaths(adminJs, false, false, true).ToTags();
            var pageScriptTags = resourceService.GetResourcePaths(pageResources).ToTags();

            builder.AppendLine(adminScriptTags);
            builder.AppendLine(pageScriptTags);

            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        ///   Renders the css for the page
        /// </summary>
        /// <typeparam name = "TModel"></typeparam>
        /// <param name = "htmlHelper"></param>
        /// <param name = "scripts">Additional scripts supplied by the use</param>
        public static MvcHtmlString RenderCss<TModel>(this HtmlHelper<TModel> htmlHelper, params string[] userScriptPaths)
        {
            var resourceService = ObjectFactory.GetInstance<IResourceService>();

            var css = new ResourceCollection();
            // include the widget scripts for the page
            css.AddRange(htmlHelper.ViewContext.Controller.ViewBag.Css);
            // include the widget scripts for the page
            foreach (var script in userScriptPaths)
            {
                css.Load(script, MatchMode.Path);
            }

            // we treat these separately because we don't want the admin js to be mashed or cached
            var cssTags = resourceService.GetResourcePaths(css).ToTags();

            return new MvcHtmlString(cssTags);
        }

        /// <summary>
        ///   The page title
        /// </summary>
        /// <typeparam name = "TModel"></typeparam>
        /// <param name = "htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString Title<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new MvcHtmlString(htmlHelper.ViewContext.Controller.ViewBag.Page.Title);
        }

        /// <summary>
        ///   The head contents
        /// </summary>
        /// <typeparam name = "TModel"></typeparam>
        /// <param name = "htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString Head<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            var page = htmlHelper.ViewContext.Controller.ViewBag.Page as Page;
            var template = htmlHelper.ViewContext.Controller.ViewBag.Template as Template;
            var root = htmlHelper.ViewContext.Controller.ViewBag.Root as Folder;

            if (page == null)
                throw new ApplicationException("Page cannot be found");

            var builder = new StringBuilder();

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Charset, template.Charset))
            {
                var charset = new TagBuilder("meta");
                charset.Attributes["charset"] = page.Charset.Or(template.Charset);
                builder.AppendLine(charset.ToString(TagRenderMode.SelfClosing));
            }

            if (!string.IsNullOrWhiteSpace(page.Title))
            {
                var title = new TagBuilder("title");
                title.InnerHtml = page.Title;
                builder.AppendLine(title.ToString(TagRenderMode.Normal));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Description, template.Description))
            {
                var description = new TagBuilder("meta");
                description.Attributes["name"] = "description";
                description.Attributes["content"] = page.Description.Or(template.Description);
                builder.AppendLine(description.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Keywords, template.Keywords))
            {
                var keywords = new TagBuilder("meta");
                keywords.Attributes["name"] = "keywords";
                keywords.Attributes["content"] = page.Keywords.Or(template.Keywords);
                builder.AppendLine(keywords.ToString(TagRenderMode.SelfClosing));
            }


            var date = new TagBuilder("meta");
            date.Attributes["http-equiv"] = "date";
            date.Attributes["content"] = page.Created.ToString("R");
            builder.AppendLine(date.ToString(TagRenderMode.SelfClosing));

            if (page.LastModified.HasValue)
            {
                var modified = new TagBuilder("meta");
                modified.Attributes["http-equiv"] = "last-modified";
                modified.Attributes["content"] = page.LastModified.Value.ToString("R");
                builder.AppendLine(modified.ToString(TagRenderMode.SelfClosing));
            }

            if (page.Language.HasValue)
            {
                var language = new TagBuilder("meta");
                language.Attributes["http-equiv"] = "content-language";
                language.Attributes["content"] = new CultureInfo(page.Language.Value).TwoLetterISOLanguageName;
                builder.AppendLine(language.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Author, template.Author))
            {
                var author = new TagBuilder("meta");
                author.Attributes["name"] = "author";
                author.Attributes["content"] = page.Author.Or(template.Author);
                builder.AppendLine(author.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Copyright, template.Copyright))
            {
                var copyright = new TagBuilder("meta");
                copyright.Attributes["name"] = "copyright";
                copyright.Attributes["content"] = page.Copyright.Or(template.Copyright);
                builder.AppendLine(copyright.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Robots, template.Robots))
            {
                var robots = new TagBuilder("meta");
                robots.Attributes["name"] = "robots";
                robots.Attributes["content"] = page.Robots.Or(template.Robots);
                builder.AppendLine(robots.ToString(TagRenderMode.SelfClosing));
            }

            //facebook/open graph attributes
            if (!string.IsNullOrWhiteSpace(page.Title))
            {
                var ogTitle = new TagBuilder("meta");
                ogTitle.Attributes["property"] = "og:title";
                ogTitle.Attributes["content"] = page.Title;
                builder.AppendLine(ogTitle.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgType, template.OgType))
            {
                var ogType = new TagBuilder("meta");
                ogType.Attributes["property"] = "og:type";
                ogType.Attributes["content"] = page.OgType.Or(template.OgType);
                builder.AppendLine(ogType.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.Description, template.Description))
            {
                var ogDescription = new TagBuilder("meta");
                ogDescription.Attributes["property"] = "og:description";
                ogDescription.Attributes["content"] = page.Description.Or(template.Description);
                builder.AppendLine(ogDescription.ToString(TagRenderMode.SelfClosing));
            }

            if (htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url != null)
            {
                var ogUrl = new TagBuilder("meta");
                ogUrl.Attributes["property"] = "og:url";
                ogUrl.Attributes["content"] =
                    htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.AbsolutePath.ToLowerInvariant().
                        ToFullyQualifiedUrl();
                builder.AppendLine(ogUrl.ToString(TagRenderMode.SelfClosing));
            }

            if (root != null && !string.IsNullOrWhiteSpace(root.Name))
            {
                var ogSiteName = new TagBuilder("meta");
                ogSiteName.Attributes["property"] = " og:site_name";
                ogSiteName.Attributes["content"] = root.Name;
                builder.AppendLine(ogSiteName.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgImage, template.OgImage))
            {
                var ogImage = new TagBuilder("meta");
                ogImage.Attributes["property"] = " og:image";
                ogImage.Attributes["content"] = page.OgImage.Or(template.OgImage);
                builder.AppendLine(ogImage.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgEmail, template.OgEmail))
            {
                var ogEmail = new TagBuilder("meta");
                ogEmail.Attributes["property"] = "og:email";
                ogEmail.Attributes["content"] = page.OgEmail.Or(template.OgEmail);
                builder.AppendLine(ogEmail.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgPhone, template.OgPhone))
            {
                var ogPhone = new TagBuilder("meta");
                ogPhone.Attributes["property"] = "og:phone_number";
                ogPhone.Attributes["content"] = page.OgPhone.Or(template.OgPhone);
                builder.AppendLine(ogPhone.ToString(TagRenderMode.SelfClosing));
            }

            if (page.OgLongitude.HasValue || template.OgLongitude.HasValue)
            {
                var ogLong = new TagBuilder("meta");
                ogLong.Attributes["property"] = "og:longitude";
                ogLong.Attributes["content"] = page.OgLongitude.HasValue
                                                   ? page.OgLongitude.ToString()
                                                   : template.OgLongitude.ToString();
                builder.AppendLine(ogLong.ToString(TagRenderMode.SelfClosing));
            }

            if (page.OgLatitude.HasValue || template.OgLatitude.HasValue)
            {
                var ogLat = new TagBuilder("meta");
                ogLat.Attributes["property"] = "og:latitude";
                ogLat.Attributes["content"] = page.OgLatitude.HasValue
                                                  ? page.OgLatitude.ToString()
                                                  : template.OgLatitude.ToString();
                builder.AppendLine(ogLat.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgStreetAddress, template.OgStreetAddress))
            {
                var ogAddress = new TagBuilder("meta");
                ogAddress.Attributes["property"] = "og:street-address";
                ogAddress.Attributes["content"] = page.OgStreetAddress.Or(template.OgStreetAddress);
                builder.AppendLine(ogAddress.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgLocality, template.OgLocality))
            {
                var ogLocality = new TagBuilder("meta");
                ogLocality.Attributes["property"] = "og:locality";
                ogLocality.Attributes["content"] = page.OgLocality.Or(template.OgLocality);
                builder.AppendLine(ogLocality.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgRegion, template.OgRegion))
            {
                var ogRegion = new TagBuilder("meta");
                ogRegion.Attributes["property"] = "og:region";
                ogRegion.Attributes["content"] = page.OgRegion.Or(template.OgRegion);
                builder.AppendLine(ogRegion.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgPostcode, template.OgPostcode))
            {
                var ogPostcode = new TagBuilder("meta");
                ogPostcode.Attributes["property"] = "og:postal-code";
                ogPostcode.Attributes["content"] = page.OgPostcode.Or(template.OgPostcode);
                builder.AppendLine(ogPostcode.ToString(TagRenderMode.SelfClosing));
            }

            if (StringExtensions.OneIsNotNullOrWhiteSpace(page.OgCountry, template.OgCountry))
            {
                var ogCountry = new TagBuilder("meta");
                ogCountry.Attributes["property"] = "og:country-name";
                ogCountry.Attributes["content"] = page.OgCountry.Or(template.OgCountry);
                builder.AppendLine(ogCountry.ToString(TagRenderMode.SelfClosing));
            }
            return new MvcHtmlString(builder.ToString());
        }
    }
}