using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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
                    var requestContext = new RequestContext(
                        new HttpContextWrapper(HttpContext.Current),
                        new RouteData());
                    return urlHelper = new UrlHelper(requestContext);
                }
                return urlHelper;
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            /*var orphans = filterContext.Controller.ViewBag.ContentAreas as IList<ContentArea>;
            if (orphans != null && orphans.Any())
            {
                var pageService = ObjectFactory.GetInstance<IPageService>();
                foreach (var contentArea in orphans)
                {
                    pageService.DeleteContentArea(contentArea);
                }
            }*/
        }
    }
}