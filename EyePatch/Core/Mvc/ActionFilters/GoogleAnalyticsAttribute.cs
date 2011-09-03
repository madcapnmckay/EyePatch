using System.Collections.Generic;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using EyePatch.Core.Documents;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class GoogleAnalyticsAttribute : ActionFilterAttribute
    {
        protected static Regex adminPanelRegex = new Regex("</head>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //get request and response
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            //get encoding
            var encoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrWhiteSpace(encoding))
                return;

            encoding = encoding.ToUpperInvariant();


            if (encoding.Contains("DEFLATE") || encoding == "*")
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }

            var urlHelper = new UrlHelper(filterContext.RequestContext);

            var page = filterContext.RouteData.Values["epPage"] as Page;

            if (page == null || string.IsNullOrWhiteSpace(page.AnalyticsKey))
                return;

            var analytics = new ResourcePath
            {
                ContentType = "text/javascript",
                Url = urlHelper.RouteUrl("Default",  new { action = "Analytics", controller = "Content", id = page.Id }).ToLowerInvariant()
            };

            var tags = new List<ResourcePath> { analytics }.ToTags();
            response.Filter = new RegexResponseFilter(response.Filter, adminPanelRegex,
                                                      string.Format("{0}</head>", tags));
        }
    }
}