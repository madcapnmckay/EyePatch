using System.Collections.Generic;
using EyePatch.Core.Documents.Children;

namespace EyePatch.Core.Documents
{
    public class Folder : IFolderItem
    {
        public Folder(string id)
        {
            Id = id;
            Folders = new List<IFolderItem>();
            Pages = new List<PageItem>();
        }

        #region IFolderItem Members

        public string Id { get; set; }
        public string Name { get; set; }

        public IList<IFolderItem> Folders { get; set; }
        public IList<PageItem> Pages { get; set; }

        #endregion
    }
}