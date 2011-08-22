using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;

namespace EyePatch.Core.Services
{
    public interface IFolderService
    {
        Folder RootFolder { get; }

        IFolderItem Create(string title, string parentId);
        void Rename(string id, string name);
        void Move(string id, string parent);
        void Delete(string id);

        IFolderItem FindFolder(string id);
        IFolderItem FindParentFolder(string childId);

        IFolderItem FindParentFolderOfPage(string pageId, out PageItem pageItem);
    }
}