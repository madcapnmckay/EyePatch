using System;
using System.Web;
using System.Web.Routing;
using StructureMap;

namespace EyePatch.Blog
{
    public class PostRoute : Route
    {
        #region Constructors
        public PostRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        public PostRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        public PostRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public PostRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
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

            var post = blogManager.Match(httpContext.Request.Path);

            if (post == null)
                return null;

            if (post.Published <= DateTime.UtcNow || (httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin")))
            {
                var page = blogManager.Template;

                if (page == null)
                    return null;

                routeData.Values["post"] = post;
                routeData.Values["controller"] = page.Template.Controller;
                routeData.Values["action"] = page.Template.Action;
                routeData.Values["page"] = page;

                return routeData;
            }

            return null;
        }
    }
}