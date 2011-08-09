using System.Web.Mvc;
using EyePatch.Core.Models;
using EyePatch.Core.Models.Forms;
using Newtonsoft.Json;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TemplateController : BaseController
    {
        public TemplateController(IContentManager contentManager)
            : base(contentManager)
        {
        }


        [HttpPost]
        public JsonResult Info(int id)
        {
            return JsonNet(new { success = true, data = contentManager.Template.Load(id).ToViewModel() }, NullValueHandling.Include);
        }

        [HttpPost]
        public JsonResult SearchInfo(int id)
        {
            return JsonNet(new { success = true, data = contentManager.Template.Load(id).ToSearchViewModel() });
        }

        [HttpPost]
        public JsonResult FacebookInfo(int id)
        {
            return JsonNet(new { success = true, data = contentManager.Template.Load(id).ToFacebookViewModel() });
        }

        [HttpPost]
        public JsonResult Update(TemplateForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Template.Update(form);
                return JsonNet(new { success = true, });
            }
            return JsonNet(new { success = false, });
        }

        [HttpPost]
        public JsonResult UpdateSearch(SearchForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Template.Update(form);
                return JsonNet(new { success = true, });
            }
            return JsonNet(new { success = false, });
        }

        [HttpPost]
        public JsonResult UpdateFacebook(FacebookForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Template.Update(form);
                return JsonNet(new { success = true, });
            }
            return JsonNet(new { success = false, });
        }
    }
}