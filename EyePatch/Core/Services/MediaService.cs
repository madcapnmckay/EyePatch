using System.IO;
using System.Web;

namespace EyePatch.Core.Services
{
    public class MediaService : IMediaService
    {
        #region IMediaService Members

        public DirectoryInfo CreateFolder(string url)
        {
            var path = HttpContext.Current.Server.MapPath(url);
            if (Directory.Exists(path))
                return new DirectoryInfo(path);

            return Directory.CreateDirectory(path);
        }

        public void RenameFolder(string url, string name)
        {
            var path = HttpContext.Current.Server.MapPath(url);
            if (Directory.Exists(path))
                Directory.Move(path, Path.Combine(Directory.GetParent(path).FullName, name));
        }

        public void DeleteFolder(string url)
        {
            var path = HttpContext.Current.Server.MapPath(url);
            Directory.Delete(path, true);
        }

        public void DeleteImage(string url)
        {
            var path = HttpContext.Current.Server.MapPath(url);
            File.Delete(path);
        }

        #endregion
    }
}