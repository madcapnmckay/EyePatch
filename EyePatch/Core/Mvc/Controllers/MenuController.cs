using System.Web.Mvc;
using EyePatch.Core.Models.Menu;

namespace EyePatch.Core.Mvc.Controllers
{
    public class MenuController : BaseController
    {
        public MenuController(IContentManager contentManager) : base(contentManager)
        {
        }

        [HttpGet]
        public ActionResult TopLevel()
        {
            return View("SimpleMenu",
                        new MenuViewModel(contentManager.Page.LoadFromFolder(contentManager.Folder.RootFolder.Id),
                                          Request));
        }
    }
}