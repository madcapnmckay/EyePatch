using System.Linq;
using EyePatch.Core.Util.Extensions;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class FolderNode : Node
    {
        public FolderNode()
        {
            CssClass = "folder";
        }

        public FolderNode(HierarchyNode<Entity.Folder> folder) : this()
        {
            var entity = folder.Entity;
            Id = entity.ID.ToString();
            Name = entity.Name;

            folder.ChildNodes.ToList().ForEach(AddFolder);
            entity.Pages.Where(p => !p.IsHidden).ToList().ForEach(AddPage);
        }

        public FolderNode(Entity.Folder folder)
            : this()
        {
            Id = folder.ID.ToString();
            Name = folder.Name;

            folder.Pages.Where(p => !p.IsHidden).ToList().ForEach(AddPage);
        }

        private void AddFolder(HierarchyNode<Entity.Folder> folder)
        {
            Children.Add(new FolderNode(folder));
        }

        public void AddPage(Entity.Page page)
        {
            Children.Add(new PageNode(page));
        }
    }
}