using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public JsonResult Update(int id, string contents)
        {
            contentManager.Widget.Update(id, contents);
            return Json(new { success = true });
        }
    }
}