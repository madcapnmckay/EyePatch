using System.Web.Mvc;
using System.Web.Routing;
using EyePatch.Core.Mvc.Routing;

namespace EyePatch.Blog
{
    public class BlogRouteRegistry : IRouteRegistry
    {
        #region IRouteRegistry Members

        public void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapBlogRoute("EyePatchBlogPost", "{controller}/{action}");
            routes.Add("EyePatchBlogPost", new PostRoute("{slug}", new MvcRouteHandler()));
            routes.Add("PostsTagged", new TaggedRoute("tagged/{tag}", new MvcRouteHandler()));
            //routes.MapRoute("PostsTagged", "tagged/{tag}", new { controller = "Blog", action = "List" });
            routes.MapRoute("BlogRssFeed", "rss", new {controller = "Blog", action = "Feed"});
        }

        #endregion
    }
}