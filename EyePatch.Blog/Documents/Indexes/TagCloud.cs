using System.Linq;
using EyePatch.Blog.Models.Widgets;
using Raven.Client.Indexes;

namespace EyePatch.Blog.Documents.Indexes
{
    public class TagCloud : AbstractIndexCreationTask<Post, TagCloudItem>
    {
        public TagCloud()
        {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new {Tag = tag, Count = 1};

            Reduce = results => from result in results
                                group result by result.Tag
                                into g
                                select new {Tag = g.Key, Count = g.Sum(x => x.Count)};
        }
    }
}