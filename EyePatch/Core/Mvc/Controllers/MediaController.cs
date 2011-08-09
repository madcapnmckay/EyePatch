using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EyePatch.Core.Models.Tree.Nodes;
using EyePatch.Core.Util;

namespace EyePatch.Core.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MediaController : BaseController
    {
        protected static string[] fileTypes = new string[] { ".tif", ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

        public MediaController(IContentManager contentManager)
            : base(contentManager)
        {
        }

        [HttpPost]
        public JsonResult Info(string id)
        {
            var path = Server.MapPath(id);
            if (!Directory.Exists(path))
                throw new ApplicationException("Folder does not exist");

            var files =
                Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Where(
                    s => Path.GetExtension(s) != null && fileTypes.Contains(Path.GetExtension(s).ToLowerInvariant()));

            return JsonNet(new {success = true, data = files.Select(PathHelper.PhysicalToUrl) });
        }

        [HttpPost]
        public string Upload(string parentId, HttpPostedFileBase image)
        {
            if (string.IsNullOrWhiteSpace(parentId))
                throw new ApplicationException("Parent must be specified");

            if (image == null)
                throw new ApplicationException("No file received");

            var url = PathHelper.CombineUrl(parentId, image.FileName);
            var path = Server.MapPath(url);
            image.SaveAs(path);
            // firefox opens save as dialog, thinks json is a file instead just return as string, yuck!
            return string.Format("{{error: '', url: '{0}' }}", url);
        }

        [HttpPost]
        public JsonResult Add(string name, string parentId)
        {
            return JsonNet(new { success = true, data = new MediaFolderNode(contentManager.Media.CreateFolder(PathHelper.CombineUrl(parentId, name))) });
        }

        [HttpPost]
        public JsonResult Rename(string id, string name)
        {
            contentManager.Media.RenameFolder(id, name);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult Remove(string id)
        {
            contentManager.Media.DeleteFolder(id);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult RemoveImage(string id)
        {
            contentManager.Media.DeleteImage(id);
            return JsonNet(new { success = true });
        }

        [HttpPost]
        public JsonResult All()
        {
            var path = Server.MapPath("/Media/");
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(
                    s => Path.GetExtension(s) != null && fileTypes.Contains(Path.GetExtension(s).ToLowerInvariant())).OrderByDescending(System.IO.File.GetLastWriteTime);

            return JsonNet(new { success = true, images = files.Select(PathHelper.PhysicalToUrl) });
        }
    }
}