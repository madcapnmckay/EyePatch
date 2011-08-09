using NKnockoutUI.ContextMenu;

namespace EyePatch.Core.Models
{
    public class PageContextMenu: ContextMenu
    {
        public PageContextMenu()
            : base("page", 190)
        {
            Item("Delete", "ep.tree.page.context.nodeDelete")
                .Item("Rename", "ep.tree.page.context.nodeRename");
        }
    }
}