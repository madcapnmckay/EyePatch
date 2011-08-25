using System.Web.Mvc;
using EyePatch.Core.Models;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WidgetController : BaseController
    {
        public WidgetController(IContentManager contentManager)
            : base(contentManager)
        {
        }

        [HttpPost]
        public JsonResult Add(string pageId, string widgetId, string contentAreaId, int position, string sourceUrl)
        {
            var pageJs = contentManager.Page.Js(pageId);
            var pageCss = contentManager.Page.Css(pageId);

            return
                JsonNet(
                    new
                        {
                            success = true,
                            widget =
                        contentManager.Widget.Add(pageId, widgetId, contentAreaId, position).ToViewModel(pageJs, pageCss, contentManager, this, sourceUrl) });
        }

        [HttpPost]
        public JsonResult Remove(string pageId, string widgetId)
        {
            contentManager.Widget.Delete(pageId, widgetId);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Move(string pageId, string widgetId, string contentAreaId, int position)
        {
            contentManager.Widget.Move(pageId, widgetId, contentAreaId, position);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Sort(string pageId, string widgetId, int position)
        {
            contentManager.Widget.Sort(pageId, widgetId, position);
            return JsonNet(new { success = true });
        }
    }
}