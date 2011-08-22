using EyePatch.Blog.Documents;
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
            Id = post.Id;
            Name = post.Name;
        }
    }
}