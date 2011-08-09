using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace EyePatch.Blog.Util.Extensions
{
    public static class BlogRouteExtensions
    {
        public static Route MapBlogRoute(this RouteCollection routes, string name, string url)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            var route2 = new PostRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary((object)null),
                Constraints = new RouteValueDictionary((object)null),
                DataTokens = new RouteValueDictionary(),
            };

            routes.Add(name, route2);
            return route2;
        }
    }
}