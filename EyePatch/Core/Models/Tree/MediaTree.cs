using EyePatch.Core.Util;
using NKnockoutUI.ContextMenu;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree
{
    public class MediaTree : NKnockoutUI.Tree.Tree
    {
        public MediaTree()
        {
            Id = EyePatchApplication.SiteID + "EyePatchMeidaTree";
            Remember = true;

            Defaults = new MediatTreeDefaults();

            ContextMenu = new ContextMenuGroup("ep.buildMediaTreeContext");
            ContextMenu.CssClass = "eyepatch-admin-context";
            ContextMenu.ContextMenus.Add(new MediaFolderContextMenu());
        }

        private class MediatTreeDefaults
        {
            public Behavior MediaFolder
            {
                get
                {
                    return new Behavior { IsDraggable = false, IsDropTarget = false, CanAddChildren = true, Name = "New Folder" };
                }
            }
        }
    }
}