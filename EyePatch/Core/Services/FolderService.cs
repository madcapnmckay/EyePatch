using System;
using System.Collections.Generic;
using System.Linq;
using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using Raven.Client;

namespace EyePatch.Core.Services
{
    public class FolderService : ServiceBase, IFolderService
    {
        public FolderService(IDocumentSession session) : base(session)
        {
        }

        #region IFolderService Members

        public Folder RootFolder
        {
            get { return session.Load<Folder>("folders-1"); }
        }

        public IFolderItem Create(string name, string parentID)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            IFolderItem result;

            if (parentID == null)
            {
                // create the root
                var root = new Folder("folders-1") {Name = name};
                session.Store(root);
                result = root;
            }
            else
            {
                // find the nodes parent
                var parent = FindParentFolder(parentID);
                var folder = new FolderItem {Name = name, Id = Guid.NewGuid().ToString()};

                result = folder;

                parent.Folders.Add(folder);
            }
            session.SaveChanges();

            return result;
        }

        public void Rename(string id, string name)
        {
            var folder = FindFolder(RootFolder, id);
            folder.Name = name;
            session.SaveChanges();
        }

        public void Move(string id, string parentId)
        {
            if (id == RootFolder.Id)
                throw new ApplicationException("You cannot move the root folder");

            var oldParent = FindParentFolder(id);
            var folder = oldParent.Folders.Single(f => f.Id == id);
            var newParent = FindFolder(parentId);

            oldParent.Folders.Remove(folder);
            newParent.Folders.Add(folder);
            session.SaveChanges();
        }

        public void Delete(string id)
        {
            if (id == RootFolder.Id)
                throw new ApplicationException("You cannot delete the root folder");

            var parent = FindParentFolder(id);
            var folder = parent.Folders.Single(f => f.Id == id);

            // delete all pages
            session.Load<Page>(GetPageIds(folder)).ToList().ForEach(p => session.Delete(p));
            // delete the folder
            parent.Folders.Remove(folder);
            session.SaveChanges();
        }

        public IFolderItem FindFolder(string id)
        {
            var result = FindFolder(RootFolder, id);

            if (result == null)
                throw new ApplicationException("Folder not found");

            return result;
        }

        public IFolderItem FindParentFolder(string childId)
        {
            if (childId == RootFolder.Id)
                return RootFolder;

            var result = FindParentFolder(RootFolder, childId);

            if (result == null)
                throw new ApplicationException("Folder not found");

            return result;
        }

        #endregion

        protected IList<string> GetPageIds(IFolderItem folderItem)
        {
            var result = new List<string>();
            result.AddRange(folderItem.Pages.Select(p => p.Id));

            foreach (var folder in folderItem.Folders)
            {
                result.AddRange(GetPageIds(folder));
            }

            return result;
        }

        protected IFolderItem FindFolder(IFolderItem folderItem, string id)
        {
            if (folderItem.Id == id)
                return folderItem;

            foreach (var folder in folderItem.Folders)
            {
                var result = FindFolder(folder, id);

                if (result != null)
                    return result;
            }
            return null;
        }

        protected IFolderItem FindParentFolder(IFolderItem folderItem, string childId)
        {
            foreach (var folder in folderItem.Folders)
            {
                if (childId == folder.Id)
                    return folderItem;

                var result = FindParentFolder(folder, childId);

                if (result != null)
                    return result;
            }
            return null;
        }

        public IFolderItem FindParentFolderOfPage(string pageId, out PageItem pageItem)
        {
            var result = FindParentFolderOfPage(RootFolder, pageId, out pageItem);

            if (result == null)
                throw new ApplicationException("Page not found");

            return result;
        }

        protected IFolderItem FindParentFolderOfPage(IFolderItem folderItem, string pageId, out PageItem pageItem)
        {
            pageItem = null;
            var page = folderItem.Pages.SingleOrDefault(p => p.Id == pageId);
            if (page != null)
            {
                pageItem = page;
                return folderItem;
            }

            foreach (var folder in folderItem.Folders)
            {
                var result = FindParentFolderOfPage(folder, pageId, out pageItem);

                if (result != null)
                    return result;
            }
            return null;
        }
    }
}