using System;
using System.Web;
using System.Web.Routing;
using EyePatch.Core;
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

        public PostRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                         IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        public PostRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
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

            var post = blogManager.Match(httpContext.Request.Path);

            // return if we don't find
            if (post == null)
                return null;

            // if we found but the post isn't published and we aren't logged in
            if ((!post.Published.HasValue || post.Published > DateTime.UtcNow) && (!httpContext.User.Identity.IsAuthenticated || !httpContext.User.IsInRole("Admin")))
                return null;

            if (post.Published <= DateTime.UtcNow ||
                (httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin")))
            {
                var page = blogManager.PostTemplate;

                if (page == null)
                    return null;

                var template = contentManager.Template.Load(page.TemplateId);

                if (template == null)
                    return null;

                // set title/modified time
                page.Title = post.Title;
                if (post.Published.HasValue)
                    page.Created = post.Published.Value;
                page.LastModified = post.LastModified;

                routeData.Values["epPost"] = post;
                routeData.Values["controller"] = template.Controller;
                routeData.Values["action"] = template.Action;
                routeData.Values["epPage"] = page;
                routeData.Values["epTemplate"] = template;

                return routeData;
            }

            return null;
        }
    }
}