using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using EyePatch.Core.Mvc.ActionFilters;
using EyePatch.Core.Mvc.ActionResults;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Util.Extensions;
using Page = EyePatch.Core.Entity.Page;

namespace EyePatch.Core.Mvc.Controllers
{
    public class ContentController : BaseController
    {
        protected static Regex adminPanelRegex = new Regex("</body>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ContentController(IContentManager contentManager) : base(contentManager) { }

        
        /// <summary>
        /// The main service method for every page
        /// This method receives a page object from the routing mechanism and returns the correct view template
        /// populated with the correct widget instances, scripts and css.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Put)]
        [Compress(Order = 1)]
        //[ContentAreaCleanup (Order = 2)]
        [GoogleAnalytics(Order = 3)]
        //[CacheDependency("EyePatchOutputCache", Order = 4)]
        [OutputCache(Duration = 60, VaryByParam = "*", Order = 5)]
        //[PageOutputCache(Duration = 60)]
        public ViewResult Service(Page page)
        {
            var u = Url.ActionSeo("Panel", "Admin");

            var adminJs = new ResourceCollection();
            adminJs.Load(Resources.Resources.jQuery)
                .Load(Url.RouteUrl("AdminPanel", new { pageId = page.ID }).ToLowerInvariant(), MatchMode.Path, "text/javascript");

            ViewBag.ContentAreas = page.ContentAreas.ToList();
            ViewBag.Page = page;
            ViewBag.Js = contentManager.Page.Js(page.ID);
            ViewBag.AdminJs = adminJs;
            ViewBag.Css = contentManager.Page.Css(page.ID);

            // add cache dependency so we can do sitewide clears on page change
            contentManager.Page.AddOutputCacheDependency(((HttpApplication)HttpContext.GetService(typeof(HttpApplication))).Context);

            return View(Path.GetFileNameWithoutExtension(page.Template.ViewPath));
        }

        /// <summary>
        /// Generates the robots.txt for the site
        /// </summary>
        /// <returns></returns>
        public ViewResult Robots()
        {
            return View();
        }

        /// <summary>
        /// Generates the analytics code to track the site if it exists
        /// </summary>
        /// <param name="id">The template id from which to pull the analytics key</param>
        /// <returns></returns>
        public ViewResult Analytics(int id)
        {
            var template = contentManager.Template.Load(id);
            if (!string.IsNullOrWhiteSpace(template.AnalyticsKey))
            {
                ViewBag.AnalyticsKey = template.AnalyticsKey;
                return View("GoogleAnalytics");
            }
            throw new ApplicationException("Analytics load failed");
        }

        /// <summary>
        /// Retrieves all the sitemap nodes for the site
        /// Scrapes all plugins and asks them to provide any nodes they wish to be includes in the sitemap
        /// </summary>
        /// <returns></returns>
        public XmlSiteMapResult SiteMap()
        {
            var allNodes = new List<ISiteMapItem>();

            // all pages, excluding hidden/dynamic
            allNodes.AddRange(contentManager.Page.All().Select(p => p.Value));

            allNodes.AddRange(contentManager.Plugin.SiteMapItems());

            return new XmlSiteMapResult(allNodes);
        }
    }
}