using EyePatch.Core.Documents;
using EyePatch.Core.Models.Tree.Nodes;
using EyePatch.Core.Util;
using NKnockoutUI.ContextMenu;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree
{
    public class PageTree : NKnockoutUI.Tree.Tree
    {
        public PageTree(Folder root)
        {
            Id = EyePatchApplication.SiteID + "EyePatchPageTree";
            Remember = true;
            Defaults = new PageTreeDefaults();

            ContextMenu = new ContextMenuGroup("ep.buildPageTreeContext");
            ContextMenu.CssClass = "eyepatch-admin-context";
            ContextMenu.ContextMenus.Add(new FolderContextMenu());
            ContextMenu.ContextMenus.Add(new PageContextMenu());

            Children.Add(new FolderNode(root));
        }

        #region Nested type: PageTreeDefaults

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
                get { return new Behavior {ChildType = "page", Name = "New Folder"}; }
            }
        }

        #endregion
    }
}