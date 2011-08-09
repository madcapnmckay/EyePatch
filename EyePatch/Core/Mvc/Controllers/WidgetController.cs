using System.Web.Mvc;
using EyePatch.Core.Models;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WidgetController : BaseController
    {
        public WidgetController(IContentManager contentManager) : base(contentManager)
        {
        }

        [HttpPost]
        public JsonResult Add(int pageId, int widgetId, int contentAreaId, int position)
        {
            return
                JsonNet(
                    new { success = true, widget = contentManager.Widget.Add(pageId, widgetId, contentAreaId, position).ToViewModel(contentManager, this) });
        }

        [HttpPost]
        public JsonResult Remove(int instanceId)
        {
            contentManager.Widget.Delete(instanceId);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Move(int instanceId, int contentAreaId, int position)
        {
            contentManager.Widget.Move(instanceId, contentAreaId, position);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Sort(int instanceId, int position)
        {
            contentManager.Widget.Sort(instanceId, position);
            return JsonNet(new { success = true });
        }
    }
}