using System.IO;

namespace EyePatch.Core.Services
{
    public interface IMediaService
    {
        DirectoryInfo CreateFolder(string url);
        void RenameFolder(string url, string name);
        void DeleteFolder(string url);
        void DeleteImage(string url);
    }
}