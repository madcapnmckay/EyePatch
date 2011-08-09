using System;
using System.Web.Mvc;
using EyePatch.Core.Models;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Models.Tree.Nodes;
using Newtonsoft.Json;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageController : BaseController
    {
        public PageController(IContentManager contentManager) : base(contentManager)
        {
        }

        [HttpPost]
        public JsonResult Info(int id)
        {
            return JsonNet(new { success = true, data = contentManager.Page.Load(id).ToViewModel(contentManager.Page.Homepage()) });
        }

        [HttpPost]
        public JsonResult SearchInfo(int id)
        {
            return JsonNet(new { success = true, data = contentManager.Page.Load(id).ToSearchViewModel() }, NullValueHandling.Include);
        }

        [HttpPost]
        public JsonResult FacebookInfo(int id)
        {
            return JsonNet(new { success = true, data = contentManager.Page.Load(id).ToFacebookViewModel() }, NullValueHandling.Include);
        }

        [HttpPost]
        public JsonResult Update(PageForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Page.Update(form);
                return JsonNet(new {success = true,});
            }
            return JsonNet(new { success = false, });
        }

        [HttpPost]
        public JsonResult UpdateSearch(SearchForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Page.Update(form);
                return JsonNet(new { success = true, });
            }
            return JsonNet(new { success = false, });
        }

        [HttpPost]
        public JsonResult UpdateFacebook(FacebookForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Page.Update(form);
                return JsonNet(new { success = true, });
            }
            return JsonNet(new { success = false, });
        }

        [HttpPost]
        public JsonResult Add(string name, int parentId)
        {
            var newPage = contentManager.Page.Create(name, name, string.Empty, parentId, false);
            return JsonNet(new { success = true, data = new PageNode(newPage) });
        }

        [HttpPost]
        public JsonResult Rename(int id, string name)
        {
            contentManager.Page.Rename(id, name);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Remove(int id)
        {
            contentManager.Page.Delete(id);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Move(int id, int parentId)
        {
            contentManager.Page.Move(id, parentId);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Navigate(int id)
        {
            var page = contentManager.Page.Load(id);
            return JsonNet(new {success = true, url = page.Url });
        }
    }
}