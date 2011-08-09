using System.Collections.Generic;
using EyePatch.Core.Entity;

namespace EyePatch.Core.Services
{
    public interface IFolderService
    {
        IList<Folder> All();

        Folder RootFolder { get; }
        Folder Load(int id);

        Folder Create(string title, int parentId);
        void Rename(int id, string name);
        void Move(int id, int parent);
        void Delete(int id);
    }
}