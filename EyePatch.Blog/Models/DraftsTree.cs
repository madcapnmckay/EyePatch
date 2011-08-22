using EyePatch.Blog.Documents;
using EyePatch.Core.Util;
using NKnockoutUI.ContextMenu;
using NKnockoutUI.Tree;

namespace EyePatch.Blog.Models
{
    public class DraftsTree : Tree
    {
        public DraftsTree()
        {
            Id = EyePatchApplication.SiteID + "EyePatchBlogDraftsTree";
            Remember = true;
            Defaults = new DraftsTreeDefaults();

            ContextMenu = new ContextMenuGroup("ep.blog.buildPostTreeContext");
            ContextMenu.CssClass = "eyepatch-admin-context";
            ContextMenu.ContextMenus.Add(new PostContextMenu());
        }

        public void AddPost(Post post)
        {
            Children.Add(new PostNode(post));
        }

        #region Nested type: DraftsTreeDefaults

        private class DraftsTreeDefaults : Behavior
        {
            public DraftsTreeDefaults()
            {
                ChildType = "post";
                Name = "New Post";
            }

            public Behavior Post
            {
                get
                {
                    return new Behavior
                               {IsDraggable = false, IsDropTarget = false, CanAddChildren = false, Name = "New Post"};
                }
            }
        }

        #endregion
    }
}