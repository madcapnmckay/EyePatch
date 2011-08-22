using System.Web.Mvc;
using EyePatch.Core.Documents.Children;
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
        public JsonResult Info(string id)
        {
            return JsonNet(new {success = true, data = contentManager.Page.Load(id).ToViewModel()});
        }

        [HttpPost]
        public JsonResult SearchInfo(string id)
        {
            return
                JsonNet(
                    new {success = true, data = contentManager.Page.Load(id).ToSearchViewModel(contentManager.Template)},
                    NullValueHandling.Include);
        }

        [HttpPost]
        public JsonResult FacebookInfo(string id)
        {
            return
                JsonNet(
                    new
                        {
                            success = true,
                            data = contentManager.Page.Load(id).ToFacebookViewModel(contentManager.Template)
                        },
                    NullValueHandling.Include);
        }

        [HttpPost]
        public JsonResult Update(PageForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Page.Update(form);
                return JsonNet(new {success = true,});
            }
            return JsonNet(new {success = false,});
        }

        [HttpPost]
        public JsonResult UpdateSearch(SearchForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Page.Update(form);
                return JsonNet(new {success = true,});
            }
            return JsonNet(new {success = false,});
        }

        [HttpPost]
        public JsonResult UpdateFacebook(FacebookForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Page.Update(form);
                return JsonNet(new {success = true,});
            }
            return JsonNet(new {success = false,});
        }

        [HttpPost]
        public JsonResult Add(string name, string parentId)
        {
            var newPage = contentManager.Page.Create(name, name, string.Empty, parentId, false);
            return
                JsonNet(
                    new
                        {
                            success = true,
                            data = new PageNode(new PageItem {Id = newPage.Id, IsHomePage = false, Name = newPage.Name})
                        });
        }

        [HttpPost]
        public JsonResult Rename(string id, string name)
        {
            contentManager.Page.Rename(id, name);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Remove(string id)
        {
            contentManager.Page.Delete(id);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Move(string id, string parentId)
        {
            contentManager.Page.Move(id, parentId);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Navigate(string id)
        {
            var page = contentManager.Page.Load(id);
            return JsonNet(new {success = true, url = page.Url});
        }
    }
}