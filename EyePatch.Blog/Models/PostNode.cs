using EyePatch.Blog.Entity;
using NKnockoutUI.Tree;

namespace EyePatch.Blog.Models
{
    public class PostNode : Node
    {
        public PostNode()
        {
            CssClass = "post";
        }

        public PostNode(Post post)
            : this()
        {
            Id = post.ID.ToString();
            Name = post.Name;
        }
    }
}