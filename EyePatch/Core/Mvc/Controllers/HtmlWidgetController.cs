using System.Web.Mvc;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HtmlWidgetController : BaseController
    {
        public HtmlWidgetController(IContentManager contentManager) : base(contentManager)
        {
        }

        [ValidateInput(false), HttpPost]
        public JsonResult Update(string pageId, string id, string contents)
        {
            contentManager.Widget.Update(pageId, id, contents);
            return Json(new {success = true});
        }
    }
}