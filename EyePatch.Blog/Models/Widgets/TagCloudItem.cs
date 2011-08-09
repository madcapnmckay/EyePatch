using System.Linq;
using System.Web.Mvc;
using EyePatch.Blog.Entity;

namespace EyePatch.Blog.Models.Widgets
{
    public class TagCloudItem
    {
        public string Tag { get; protected set; }
        public string TagTitle { get { return string.Format("Click to view posts tagged {0}", Tag); } }
        public int Count { get; protected set; }

        public string TaggedLink { get; protected set; }

        public TagCloudItem(Tag tag, int count, UrlHelper urlHelper)
        {
            Tag = tag.Name;
            Count = count;

            TaggedLink = urlHelper.RouteUrl("PostsTagged", new { tag = tag.Name });
        }
    }
}