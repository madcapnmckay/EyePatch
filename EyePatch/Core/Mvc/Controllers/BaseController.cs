using System.Web.Mvc;
using EyePatch.Core.Mvc.ActionResults;
using Newtonsoft.Json;

namespace EyePatch.Core.Mvc.Controllers
{
    public class BaseController : Controller
    {
        protected IContentManager contentManager;

        public BaseController(IContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        protected JsonResult JsonNet(object data)
        {
            return new JsonNetResult(data);
        }

        protected JsonResult JsonNet(object data, NullValueHandling nullValues)
        {
            return new JsonNetResult(data, nullValues);
        }

        protected JsonResult JsonNet(object data, JsonSerializerSettings settings)
        {
            return new JsonNetResult(data, settings);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is JsonNetResult)
            {
#if DEBUG
                ((JsonNetResult) filterContext.Result).Formatting = Formatting.Indented;
#endif
            }
        }
    }
}