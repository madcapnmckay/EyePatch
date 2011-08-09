using System.Web;
using System.Web.Routing;
using StructureMap;

namespace EyePatch.Blog
{
    public class TaggedRoute : Route
    {
        #region Constructors
        public TaggedRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        public TaggedRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        public TaggedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public TaggedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
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

            var blogManager = ObjectFactory.GetInstance<IBlogManager>();

            var page = blogManager.PostList;

            if (page == null)
                return null;

            routeData.Values["controller"] = page.Template.Controller;
            routeData.Values["action"] = page.Template.Action;
            routeData.Values["page"] = page;

            return routeData;
        }
    }
}