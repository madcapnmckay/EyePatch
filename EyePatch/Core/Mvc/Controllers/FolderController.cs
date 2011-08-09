
using System.Web.Mvc;
using EyePatch.Core.Models.Tree.Nodes;
using Newtonsoft.Json;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FolderController : BaseController
    {
        public FolderController(IContentManager contentManager) : base(contentManager)
        {
        }

        [HttpPost]
        public JsonResult Add(string name, int parentId)
        {
            var newFolder = contentManager.Folder.Create(name, parentId);
            return JsonNet(new { success = true, data = new FolderNode(newFolder) });
        }

        [HttpPost]
        public JsonResult Rename(int id, string name)
        {
            contentManager.Folder.Rename(id, name);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Remove(int id)
        {
            contentManager.Folder.Delete(id);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Move(int id, int parent)
        {
            contentManager.Folder.Move(id, parent);
            return JsonNet(new { success = true });
        }
    }
}