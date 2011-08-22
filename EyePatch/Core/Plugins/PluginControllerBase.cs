using System.Web.Mvc;
using EyePatch.Core.Mvc.Controllers;

namespace EyePatch.Core.Plugins
{
    public class PluginControllerBase : BaseController
    {
        public PluginControllerBase(IContentManager contentManager) : base(contentManager)
        {
        }

        private string GetPluginPath(string viewName)
        {
            var controllerName = RouteData.GetRequiredString("controller");
            var view = string.IsNullOrEmpty(viewName) ? RouteData.GetRequiredString("action") : viewName;
            var assemblyName = GetType().Assembly.FullName.Split(',')[0];
            return string.Format(@"{3}/{0}.dll/{0}.Views.{1}.{2}.cshtml", assemblyName, controllerName, view,
                                 ContentManager.PluginDir);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }

            return new ViewResult
                       {
                           ViewName = GetPluginPath(viewName),
                           MasterName = masterName,
                           ViewData = ViewData,
                           TempData = TempData
                       };
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }

            return new PartialViewResult
                       {
                           ViewName = GetPluginPath(viewName),
                           ViewData = ViewData,
                           TempData = TempData
                       };
        }
    }
}