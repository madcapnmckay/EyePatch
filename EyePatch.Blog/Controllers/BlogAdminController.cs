using System.Web.Mvc;
using EyePatch.Blog.Models;
using EyePatch.Blog.Models.Forms;
using EyePatch.Core;
using EyePatch.Core.Mvc.Controllers;

namespace EyePatch.Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogAdminController : BaseController
    {
        protected IBlogManager blogManager;

        public BlogAdminController(IContentManager contentManager, IBlogManager blogManager)
            : base(contentManager)
        {
            this.blogManager = blogManager;
        }

        [HttpPost]
        public JsonResult Remove(string id)
        {
            blogManager.Delete(id);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Info(string id)
        {
            return JsonNet(new {success = true, data = blogManager.Load(id).ToForm()});
        }

        [HttpPost]
        public JsonResult Navigate(string id)
        {
            var post = blogManager.Load(id);
            if (string.IsNullOrWhiteSpace(post.Url))
                return JsonNet(new {success = false, message = "The post does not have a url yet."});

            return JsonNet(new {success = true, url = post.Url});
        }

        [HttpPost]
        public JsonResult Create(string name)
        {
            return JsonNet(new {success = true, data = new PostNode(blogManager.Create(name))});
        }

        [HttpPost]
        public JsonResult Publish(string id)
        {
            blogManager.Publish(id);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Update(PostForm form)
        {
            if (ModelState.IsValid)
            {
                blogManager.Update(form);
                if (form.Published)
                    blogManager.Publish(form.Id);

                return JsonNet(new {success = true, published = form.Published});
            }
            return JsonNet(new {success = false});
        }

        public JsonResult Body(string postId, string html)
        {
            blogManager.UpdateBody(postId, html);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult Rename(string id, string name)
        {
            blogManager.Rename(id, name);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Settings(string listPage, string template, string disqus)
        {
            blogManager.UpdateSettings(listPage, template, disqus);
            return JsonNet(new {success = true});
        }
    }
}