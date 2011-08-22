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
        public PartialViewResult List(string tag, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return PartialView(blogManager.Posts(page, pageSize).ToViewModel(blogManager.Settings, pageSize));

            return PartialView(blogManager.Tagged(tag, page, pageSize).ToViewModel(blogManager.Settings, pageSize));
        }

        [HttpGet]
        public PartialViewResult TagCloud()
        {
            return PartialView(blogManager.TagCloud(25));
        }

        [HttpGet]
        public ActionResult Feed()
        {
            var posts = blogManager.Posts(1, 10).ToViewModel(blogManager.Settings, 10);
            return new RssResult(posts, string.Format("{0} - Latest Posts", contentManager.SiteName),
                                 contentManager.SiteDescription);
        }
    }
}