using NKnockoutUI.ContextMenu;

namespace EyePatch.Blog.Models
{
    public class PostContextMenu : ContextMenu
    {
        public PostContextMenu()
            : base("post", 190)
        {
            Item("Delete", "ep.blog.postDelete")
                .Item("Rename", "ep.blog.postRename");
        }
    }
}