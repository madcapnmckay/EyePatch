using System.Web.Mvc;
using EyePatch.Core.Mvc.ActionFilters;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Util;

namespace EyePatch.Core.Mvc.Controllers
{
    public class ResourceController : BaseController
    {
        public ResourceController(IContentManager contentManager) : base(contentManager)
        {
        }

        [/*OutputCache(Duration = 604800), */Compress]
        public FileResult Fetch(string id)
        {
            var resource = contentManager.Resources.GetContentsFor(id);

            if (resource != null)
            {
                Response.AddFileDependency(resource.FileName);
                return File(new System.Text.ASCIIEncoding().GetBytes(resource.Contents), resource.ContentType + "; charset=UTF-8");
            }
            return null;
        }

        [HttpGet, OutputCache(Duration = 604800)]
        public FileResult Embedded(string fileName)
        {
            return File(contentManager.Resources.EmbeddedResource(fileName), contentManager.Resources.MimeType(fileName));
        }
    }
}