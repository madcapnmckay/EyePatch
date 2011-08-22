using System.Web;
using System.Web.Routing;
using EyePatch.Core.Util;
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

        public ManagedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                            IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public ManagedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                            RouteValueDictionary dataTokens, IRouteHandler routeHandler)
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
            if (!EyePatchApplication.HasPages)
                return routeData;

            var page = contentManager.Page.Match(httpContext.Request.Path);

            //// return if we don't find
            if (page == null)
                return null;

            // if we found but the page isn't live and we aren't logged in
            if (!page.IsLive && (!httpContext.User.Identity.IsAuthenticated || !httpContext.User.IsInRole("Admin")))
                return null;

            var template = contentManager.Template.Load(page.TemplateId);

            if (template == null)
                return null;

            if (routeData.Values.ContainsKey("controller"))
                routeData.Values["controller"] = template.Controller;

            if (routeData.Values.ContainsKey("action"))
                routeData.Values["action"] = template.Action;

            if (routeData.Values.ContainsKey("area"))
                routeData.Values["area"] = "CMS";

            routeData.Values["page"] = page;
            routeData.Values["template"] = template;

            return routeData;
        }
    }
}