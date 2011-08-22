using System.Web;

namespace EyePatch.Core.Mvc.Resources
{
    public class ResourcePathProvider : IResourcePathProvider
    {
        #region IResourcePathProvider Members

        public virtual string MapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }

        #endregion
    }
}