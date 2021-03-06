﻿using EyePatch.Blog.Documents;
using EyePatch.Core.Util;
using NKnockoutUI.ContextMenu;
using NKnockoutUI.Tree;

namespace EyePatch.Blog.Models
{
    public class PublishedTree : Tree
    {
        public PublishedTree()
        {
            Id = EyePatchApplication.SiteID + "EyePatchBlogPublishedTree";
            Remember = true;
            Defaults = new PublishedTreeDefaults();

            ContextMenu = new ContextMenuGroup("ep.blog.buildPostTreeContext");
            ContextMenu.CssClass = "eyepatch-admin-context";
            ContextMenu.ContextMenus.Add(new PostContextMenu());
        }

        public void AddPost(Post post)
        {
            Children.Add(new PostNode(post));
        }

        #region Nested type: PublishedTreeDefaults

        private class PublishedTreeDefaults
        {
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