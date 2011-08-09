using NKnockoutUI.ContextMenu;

namespace EyePatch.Core.Models
{
    public class MediaFolderContextMenu: ContextMenu
    {
        public MediaFolderContextMenu()
            : base("folder", 190)
        {
            Item("Delete", "ep.tree.page.context.nodeDelete")
            .Item("Rename", "ep.tree.page.context.nodeRename")
            .Separator()
            .Item("New", new SubContextMenu()
                                .Item("Folder", "folder", "ep.tree.mediaFolder.context.newFolder"));
        }
    }
}