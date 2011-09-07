using System.Web;
using System.Web.Routing;
using EyePatch.Core;
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

        public TaggedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                           IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public TaggedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
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

            var blogManager = ObjectFactory.GetInstance<IBlogManager>();
            var contentManager = ObjectFactory.GetInstance<IContentManager>();

            var page = blogManager.PostList;

            if (page == null)
                return null;

            var template = contentManager.Template.Load(page.TemplateId);

            if (template == null)
                return null;

            routeData.Values["controller"] = template.Controller;
            routeData.Values["action"] = template.Action;
            routeData.Values["epPage"] = page;
            routeData.Values["epTemplate"] = template;

            return routeData;
        }
    }
}