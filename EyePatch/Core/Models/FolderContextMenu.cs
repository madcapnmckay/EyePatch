using NKnockoutUI.ContextMenu;

namespace EyePatch.Core.Models
{
    public class FolderContextMenu : ContextMenu
    {
        public FolderContextMenu()
            : base("folder", 190)
        {
            Item("Delete", "ep.tree.page.context.nodeDelete")
                .Item("Rename", "ep.tree.page.context.nodeRename")
                .Separator()
                .Item("New", new SubContextMenu()
                                 .Item("Page", "page", "ep.tree.page.context.newPage")
                                 .Item("Folder", "folder", "ep.tree.page.context.newFolder"));
        }
    }
}