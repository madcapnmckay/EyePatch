using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EyePatch.Core.Documents;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Services;
using EyePatch.Core.Util.Extensions;
using StructureMap;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class GoogleAnalyticsAttribute : ActionFilterAttribute
    {
        protected static Regex adminPanelRegex = new Regex("</head>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // bad monkey! need to inject
        protected IResourceService resourceService = ObjectFactory.GetInstance<IResourceService>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlHelper = new UrlHelper(filterContext.RequestContext);

            var page = filterContext.RouteData.Values["epPage"] as Page;

            if (page == null || string.IsNullOrWhiteSpace(page.AnalyticsKey))
                return;

            var analytics = new ResourcePath
                                {
                                    ContentType = "text/javascript",
                                    Url = urlHelper.ActionSeo("Analytics", "Content", new { id = page.Id })
                                };

            var response = filterContext.HttpContext.Response;
            response.Filter = new RegexResponseFilter(response.Filter, adminPanelRegex,
                                                      string.Format("{0}</head>", new List<ResourcePath> {analytics}.ToTags()));
        }
    }
}