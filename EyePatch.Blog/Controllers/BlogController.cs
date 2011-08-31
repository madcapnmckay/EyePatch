using System.Linq;
using System.Web.Mvc;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Util.ActionResult;
using EyePatch.Blog.Util.Extensions;
using EyePatch.Core;
using EyePatch.Core.Plugins;

namespace EyePatch.Blog.Controllers
{
    public class BlogController : PluginControllerBase
    {
        protected IBlogManager blogManager;

        public BlogController(IContentManager contentManager, IBlogManager blogManager)
            : base(contentManager)
        {
            this.blogManager = blogManager;
        }

        [HttpGet]
        public PartialViewResult Post(Post post)
        {
            if (post == null || post.Id == null)
            {
                // figure out what post we are on
                if (!RouteData.Values.ContainsKey("SourceUrl") || RouteData.Values["SourceUrl"] == null)
                    return PartialView("Invalid");

                var source = RouteData.Values["SourceUrl"].ToString();
                post = blogManager.Match(source);

                if (post == null)
                    return PartialView("Invalid");
            }

            return PartialView(post.ToViewModel(blogManager.Settings));
        }

        [HttpGet]
        public PartialViewResult List(string tag, int page = 1, int pageSize = 5)
        {
            int totalResults;
            if (string.IsNullOrWhiteSpace(tag))
                return PartialView(blogManager.Posts(page, pageSize, out totalResults).ToViewModel(blogManager.Settings, page, pageSize, totalResults));

            return PartialView(blogManager.Tagged(tag, page, pageSize, out totalResults).ToViewModel(blogManager.Settings, page, pageSize, totalResults));
        }

        [HttpGet]
        public PartialViewResult TagCloud()
        {
            return PartialView(blogManager.TagCloud(50));
        }

        [HttpGet]
        public ActionResult Feed()
        {
            int totalResults;
            var posts = blogManager.Posts(1, 10, out totalResults).ToViewModel(blogManager.Settings, 1, 10, totalResults);
            return new RssResult(posts, string.Format("{0} - Latest Posts", contentManager.SiteName),
                                 contentManager.SiteDescription);
        }
    }
}