using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using EyePatch.Core.Entity;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Services;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;
using StructureMap;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class ContentAreaCleanupAttribute : ActionFilterAttribute
    {
        protected UrlHelper urlHelper;

        public UrlHelper Url
        {
            get
            {
                if (urlHelper == null)
                {
                    var requestContext = new System.Web.Routing.RequestContext(
                                new HttpContextWrapper(HttpContext.Current),
                                new System.Web.Routing.RouteData());
                    return urlHelper = new UrlHelper(requestContext);
                }
                return urlHelper;
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var orphans = filterContext.Controller.ViewBag.ContentAreas as IList<ContentArea>;
            if (orphans != null && orphans.Any())
            {
                var pageService = ObjectFactory.GetInstance<IPageService>();
                foreach (var contentArea in orphans)
                {
                    pageService.DeleteContentArea(contentArea);
                }
            }
        }
    }
} 