using System;
using System.Collections.Generic;

namespace EyePatch.Blog.Models.Widgets
{
    public class TagCloud : List<TagCloudItem>
    {
        protected int totalPosts;

        public TagCloud(IEnumerable<TagCloudItem> items, int totalPosts)
        {
            this.totalPosts = totalPosts;
            AddRange(items);
        }

        public string CssClass(TagCloudItem tag)
        {
            var result = (tag.Count / (double)totalPosts);
            return string.Format("tag{0}", Math.Ceiling(result / 0.05));
        }
    }
}