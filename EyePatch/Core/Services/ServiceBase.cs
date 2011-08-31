using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using Raven.Client;

namespace EyePatch.Core.Services
{
    public abstract class ServiceBase
    {
        protected IDocumentSession session;
        private UrlHelper urlHelper;

        protected ServiceBase(IDocumentSession session)
        {
            this.session = session;
        }

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

        public Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }
    }
}