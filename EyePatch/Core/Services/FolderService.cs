using System;
using System.Collections.Generic;
using System.Linq;
using EyePatch.Core.Entity;

namespace EyePatch.Core.Services
{
    public class FolderService : ServiceBase, IFolderService
    {
        public FolderService(EyePatchDataContext context) : base(context) {}

        public IList<Folder> All()
        {
            return db.Folders.ToList();
        }

        public Folder RootFolder
        {
            get
            {
                return db.Folders.SingleOrDefault(f => f.ID == 1);
            }
        }

        public Folder Load(int id)
        {
            var folder = db.Folders.SingleOrDefault(f => f.ID == id);

            if (folder == null)
                throw new ApplicationException("Folder does not exist");

            return folder;
        }

        public Folder Create(string name, int parentId)
        {
            if (name == null) throw new ArgumentNullException("name");

            var parent = parentId == -1 ? RootFolder : Load(parentId);
            var folder = new Folder
            {
                Name = name,
                Parent = parent
            };
            db.Folders.InsertOnSubmit(folder);
            db.SubmitChanges();

            return folder;
        }

        public void Rename(int id, string name)
        {
            var folder = Load(id);
            folder.Name = name;
            db.SubmitChanges();
        }

        public void Move(int id, int parent)
        {
            var folder = Load(id);
            var newParent = Load(parent);

            folder.Parent = newParent;
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            var folder = Load(id);

            db.Pages.DeleteAllOnSubmit(folder.Pages);

            // TODO fix this abomination
            db.Pages.DeleteAllOnSubmit(folder.Pages);
            folder.Folders.Select(f => f.ID).ToList().ForEach(Delete);

            db.Folders.DeleteOnSubmit(folder);
            db.SubmitChanges();
        }
    }
}