using System.Collections.Generic;

namespace EyePatch.Core.Documents.Children
{
    public class FolderItem : IFolderItem
    {
        public FolderItem()
        {
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