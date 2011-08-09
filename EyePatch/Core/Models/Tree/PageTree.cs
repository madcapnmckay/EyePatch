using EyePatch.Core.Models.Tree.Nodes;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;
using NKnockoutUI.ContextMenu;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree
{
    public class PageTree : NKnockoutUI.Tree.Tree
    {
        public PageTree()
        {
            Id = EyePatchApplication.SiteID + "EyePatchPageTree";
            Remember = true;
            Defaults = new PageTreeDefaults();

            ContextMenu = new ContextMenuGroup("ep.buildPageTreeContext");
            ContextMenu.CssClass = "eyepatch-admin-context";
            ContextMenu.ContextMenus.Add(new FolderContextMenu());
            ContextMenu.ContextMenus.Add(new PageContextMenu());
        }

        public void AddFolder(HierarchyNode<Entity.Folder> folder)
        {
            Children.Add(new FolderNode(folder));
        }

        private class PageTreeDefaults
        {
            public Behavior Page
            {
                get
                {
                    return new Behavior
                               {IsDraggable = true, IsDropTarget = false, CanAddChildren = false, Name = "New Page"};
                }
            }

            public Behavior Folder
            {
                get
                {
                    return new Behavior { ChildType = "page", Name = "New Folder" };
                }
            }
        }
    }
}