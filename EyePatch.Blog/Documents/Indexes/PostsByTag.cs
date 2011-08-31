using System.Linq;
using Raven.Client.Indexes;

namespace EyePatch.Blog.Documents.Indexes
{
    public class PostsByTag : AbstractIndexCreationTask<Post>
    {
        public PostsByTag()
        {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new { tag };
        }
    }
}