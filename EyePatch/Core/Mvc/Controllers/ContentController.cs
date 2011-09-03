using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using EyePatch.Core.Documents;
using EyePatch.Core.Mvc.ActionFilters;
using EyePatch.Core.Mvc.ActionResults;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Core.Mvc.Controllers
{
    public class ContentController : BaseController
    {
        protected static Regex adminPanelRegex = new Regex("</body>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ContentController(IContentManager contentManager) : base(contentManager)
        {
        }


        /// <summary>
        ///   The main service method for every page
        ///   This method receives a page object from the routing mechanism and returns the correct view template
        ///   populated with the correct widget instances, scripts and css.
        /// </summary>
        /// <param name = "epPage"></param>
        /// <param name = "epTemplate"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Put)]
        //[ContentAreaCleanup (Order = 2)]
        [GoogleAnalytics(Order = 1)]
        [OutputCache(Duration = 60, VaryByParam = "*", Order = 2)]
        public ViewResult Service(Page epPage, Template epTemplate)
        {
            if (epPage == null)
                throw new ApplicationException("page is not specified");

            if (epTemplate == null)
                throw new ApplicationException("template is not specified");

            var u = Url.ActionSeo("Panel", "Admin");

            var adminJs = new ResourceCollection();
            adminJs.Load(Resources.Resources.jQuery)
                .Load(Url.RouteUrl("AdminPanel", new {pageId = epPage.Id}).ToLowerInvariant(), MatchMode.Path,
                      "text/javascript");

            ViewBag.ContentAreas = epPage.ContentAreas.ToList();
            ViewBag.Page = epPage;
            ViewBag.Template = epTemplate;
            ViewBag.Root = contentManager.Folder.RootFolder;
            ViewBag.Js = contentManager.Page.Js(epPage.Id);
            ViewBag.AdminJs = adminJs;
            ViewBag.Css = contentManager.Page.Css(epPage.Id);

            // add cache dependency so we can do sitewide clears on page change
            contentManager.Page.AddOutputCacheDependency(
                ((HttpApplication) HttpContext.GetService(typeof (HttpApplication))).Context);

            return View(Path.GetFileNameWithoutExtension(epTemplate.ViewPath));
        }

        /// <summary>
        ///   Generates the robots.txt for the site
        /// </summary>
        /// <returns></returns>
        public ViewResult Robots()
        {
            return View();
        }

        /// <summary>
        ///   Generates the analytics code to track the site if it exists
        /// </summary>
        /// <param name = "id">The template id from which to pull the analytics key</param>
        /// <returns></returns>
        public ViewResult Analytics(string id)
        {
            var page = contentManager.Page.Load(id);
            if (!string.IsNullOrWhiteSpace(page.AnalyticsKey))
            {
                ViewBag.AnalyticsKey = page.AnalyticsKey;
                return View("GoogleAnalytics");
            }
            throw new ApplicationException("Analytics load failed");
        }

        /// <summary>
        ///   Retrieves all the sitemap nodes for the site
        ///   Scrapes all plugins and asks them to provide any nodes they wish to be includes in the sitemap
        /// </summary>
        /// <returns></returns>
        public XmlSiteMapResult SiteMap()
        {
            var allNodes = new List<ISiteMapItem>();

            // all pages, excluding hidden/dynamic
            allNodes.AddRange(contentManager.Page.VisiblePages());
            allNodes.AddRange(contentManager.Plugin.SiteMapItems());

            return new XmlSiteMapResult(allNodes);
        }
    }
}