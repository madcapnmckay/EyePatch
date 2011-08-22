using System;
using System.Collections.Generic;
using System.Linq;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Models.Forms;

namespace EyePatch.Blog.Models
{
    public static class BlogPanelExtensions
    {
        public static PublishedTree ToPublishedTree(this IEnumerable<Post> posts)
        {
            var result = new PublishedTree();
            posts.ToList().ForEach(result.AddPost);
            return result;
        }

        public static DraftsTree ToDraftsTree(this IEnumerable<Post> posts)
        {
            var result = new DraftsTree();
            posts.ToList().ForEach(result.AddPost);
            return result;
        }

        public static PostForm ToForm(this Post post)
        {
            var result = new PostForm();
            result.Id = post.Id;
            result.Published = post.Published != null && post.Published <= DateTime.UtcNow;
            result.Title = post.Title ?? string.Empty;
            result.Url = post.Url ?? string.Empty;
            result.Tags = string.Join(", ", post.Tags.Select(t => t.Value));
            return result;
        }
    }
}