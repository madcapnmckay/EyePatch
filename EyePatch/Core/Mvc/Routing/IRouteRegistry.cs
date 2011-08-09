using System.Web.Routing;

namespace EyePatch.Core.Mvc.Routing
{
    public interface IRouteRegistry
    {
        void RegisterRoutes(RouteCollection routes);
    }
}
