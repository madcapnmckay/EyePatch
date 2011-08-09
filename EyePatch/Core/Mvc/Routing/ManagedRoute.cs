using System.Web;
using System.Web.Routing;
using EyePatch.Core.Entity;
using StructureMap;

namespace EyePatch.Core.Mvc.Routing
{
    public class ManagedRoute : Route
    {
        #region Contructors
        public ManagedRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        public ManagedRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        public ManagedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public ManagedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }
        #endregion

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = base.GetRouteData(httpContext);

            // Route itself didn't match
            if (routeData == null)
                return null;

            var contentManager = ObjectFactory.GetInstance<IContentManager>();

            // No pages in the cms, goto default route
            // Installation
            if (contentManager.Page.Count == 0)
                return routeData;

            var page = contentManager.Page.Match(httpContext.Request.Path);

            if (page == null)
                return null;

            if (routeData.Values.ContainsKey("controller"))
                routeData.Values["controller"] = page.Template.Controller;

            if (routeData.Values.ContainsKey("action"))
                routeData.Values["action"] = page.Template.Action;

            if (routeData.Values.ContainsKey("area"))
                routeData.Values["area"] = "CMS";

            routeData.Values["page"] = page;

            return routeData;
        }
    }
}