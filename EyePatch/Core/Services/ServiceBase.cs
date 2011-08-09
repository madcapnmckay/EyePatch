using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using EyePatch.Core.Entity;

namespace EyePatch.Core.Services
{
    public class ServiceBase
    {
        protected static readonly object PadLock = new object();
        protected EyePatchDataContext db;
        private UrlHelper urlHelper;

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

        public Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        } 

        public ServiceBase(EyePatchDataContext context)
        {
            db = context;
        }
    }
}