using System.Linq;
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
        public JsonResult Remove(int id)
        {
            blogManager.Delete(id);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Info(int id)
        {
            return JsonNet(new { success = true, data = blogManager.Load(id).ToForm() });
        }

        [HttpPost]
        public JsonResult Navigate(int id)
        {
            var post = blogManager.Load(id);
            return JsonNet(new { success = true, url = post.Url });
        }

        [HttpPost]
        public JsonResult Create(string name)
        {
            return JsonNet(new { success = true, data = new PostNode(blogManager.Create(name)) });
        }

        [HttpPost]
        public JsonResult Publish(int id)
        {
            blogManager.Publish(id);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Update(PostForm form)
        {
            if (ModelState.IsValid)
            {
                blogManager.Update(form);
                if (form.Published)
                    blogManager.Publish(form.Id);

                return JsonNet(new {success = true, published = form.Published });
            }
            return JsonNet(new { success = false });
        }

        [HttpPost]
        public JsonResult Rename(int id, string name)
        {
            blogManager.Rename(id, name);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Settings(int listPage, int template, string disqus)
        {
            blogManager.UpdateSettings(listPage, template, disqus);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Tags(string q)
        {
            return JsonNet(blogManager.TagContaining(q).Select(t => new { value = t.Name }));
        }
    }
}