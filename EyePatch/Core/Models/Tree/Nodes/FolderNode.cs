using System.Linq;
using EyePatch.Core.Documents.Children;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class FolderNode : Node
    {
        public FolderNode()
        {
            CssClass = "folder";
        }

        public FolderNode(IFolderItem folder)
            : this()
        {
            Id = folder.Id;
            Name = folder.Name;

            folder.Folders.ToList().ForEach(AddFolder);
            folder.Pages.ToList().ForEach(AddPage);
        }

        private void AddFolder(IFolderItem folder)
        {
            Children.Add(new FolderNode(folder));
        }

        public void AddPage(PageItem page)
        {
            Children.Add(new PageNode(page));
        }
    }
}