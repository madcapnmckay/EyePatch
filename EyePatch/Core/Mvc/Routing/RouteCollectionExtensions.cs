using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace EyePatch.Core.Mvc.Routing
{
    public static class RouteCollectionExtensions
    {
        public static Route MapEyePatchRoute(this RouteCollection routes, string name, string url, object defaults,
                                             object constraints, string[] namespaces)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");
            if (url == null)
                throw new ArgumentNullException("url");

            var route2 = new ManagedRoute(url, new MvcRouteHandler())
                             {
                                 Defaults = new RouteValueDictionary(defaults),
                                 Constraints = new RouteValueDictionary(constraints),
                                 DataTokens = new RouteValueDictionary(),
                             };

            if ((namespaces != null) && (namespaces.Length > 0))
                route2.DataTokens["Namespaces"] = namespaces;

            routes.Add(name, route2);
            return route2;
        }
    }
}