using System.Collections.Generic;
using System.Linq;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Models.Widgets;

namespace EyePatch.Blog.Util.Extensions
{
    public static class ViewModelExtensions
    {
        public static PostList ToViewModel(this IEnumerable<Post> posts, Documents.Blog blog, int page, int pageSize, int totalRecords)
        {
            return new PostList(page, pageSize, totalRecords, posts.Select((post, idx) => new PostSummary(post, blog, idx, pageSize)));
        }

        public static PostBody ToViewModel(this Post post, Documents.Blog blog)
        {
            return new PostBody(post, blog);
        }
    }
}