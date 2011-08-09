using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EyePatch.Blog.Entity;
using EyePatch.Blog.Models;
using EyePatch.Blog.Models.Widgets;

namespace EyePatch.Blog.Util.Extensions
{
    public static class ViewModelExtensions
    {
        public static IList<PostSummary> ToViewModel(this IList<Post> posts, BlogInfo blogInfo, int pageSize)
        {
            return posts.Select((post, idx) => new PostSummary(post, blogInfo, idx, pageSize)).ToList();
        }

        public static IList<TagCloudItem> ToViewModel(this IList<KeyValuePair<Tag, int>> tags, UrlHelper urlHelper)
        {
            return tags.Select(t => new TagCloudItem(t.Key, t.Value, urlHelper)).ToList();
        }

        public static PostBody ToViewModel(this Post post, BlogInfo blogInfo)
        {
            return new PostBody(post, blogInfo);
        }
    }
}