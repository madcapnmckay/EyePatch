using System.Collections.Generic;

namespace EyePatch.Core.Documents.Children
{
    public interface IFolderItem
    {
        string Id { get; set; }
        string Name { get; set; }

        IList<IFolderItem> Folders { get; set; }
        IList<PageItem> Pages { get; set; }
    }
}