using System;
using System.Web.Mvc;
using EyePatch.Core.Models;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Mvc.ActionFilters;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Util;

namespace EyePatch.Core.Mvc.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(IContentManager contentManager) : base(contentManager) { }

        [HttpGet]
        public ViewResult Install()
        {
            // TODO Need to redirect post install
            return View();
        }

        /// <summary>
        /// Receives the data from the user and installs EyePatch
        /// Creates the default items (page, template) etc and signs the user in
        /// </summary>
        /// <param name="install"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Install(InstallationForm install)
        {
            // TODO Need to secure post install

            if (ModelState.IsValid)
            {
                contentManager.Application.Install(install);
                return JsonNet(new {success = true});
            }
            return JsonNet(new { success = false, message = "Validation Error" });
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            return View("SignIn");
        }

        [HttpPost]
        public JsonResult SignIn(SignInForm form)
        {
            if (ModelState.IsValid)
            {
                contentManager.Application.SignIn(form.UserName, form.Password);
                return JsonNet(new { success = true });
            }
            return JsonNet(new { success = false, message = "Validation Error" });
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            contentManager.Application.SignOut();
            return Redirect("/");
        }

        /// <summary>
        /// This method returns the admin panel script file
        /// Acts as a way to inject contextually into the same html (even if output cached)
        /// If a user is authenticated they will receive the full admin panel script
        /// if not they will receive a string
        /// </summary>
        /// <returns></returns>
        [HttpGet, Compress]
        public ActionResult Panel(int pageId)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
                return Content("// Not for your eyes :)");

            ViewBag.PageId = pageId;
            return View("Bootstrapper");
        }

        /// <summary>
        /// This method returns the admin panel data used to construct the interface
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Data(int pageId)
        {
            return JsonNet(new { success = true, vm = contentManager.AdminPanel(pageId) });
        }

        /// <summary>
        /// This method returns the mashed css files for the interface
        /// </summary>
        /// <returns></returns>
        [Compress]
        public ActionResult Css()
        {
            return EyePatchApplication.ReleaseMode == ReleaseMode.Production
                       ? File("/core/css/eyepatch-admin.min.css", "text/css")
                       : File("/core/css/eyepatch-admin.css", "text/css");
        }
    }
}