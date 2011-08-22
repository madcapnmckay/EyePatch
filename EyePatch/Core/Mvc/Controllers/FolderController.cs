using System.Web.Mvc;
using EyePatch.Core.Models.Tree.Nodes;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FolderController : BaseController
    {
        public FolderController(IContentManager contentManager) : base(contentManager)
        {
        }

        [HttpPost]
        public JsonResult Add(string name, string parentId)
        {
            var newFolder = contentManager.Folder.Create(name, parentId);
            return JsonNet(new {success = true, data = new FolderNode(newFolder)});
        }

        [HttpPost]
        public JsonResult Rename(string id, string name)
        {
            contentManager.Folder.Rename(id, name);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Remove(string id)
        {
            contentManager.Folder.Delete(id);
            return JsonNet(new {success = true});
        }

        [HttpPost]
        public JsonResult Move(string id, string parentId)
        {
            contentManager.Folder.Move(id, parentId);
            return JsonNet(new {success = true});
        }
    }
}