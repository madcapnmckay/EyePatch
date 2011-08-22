using System.Linq;
using Raven.Client.Indexes;

namespace EyePatch.Blog.Documents.Indexes
{
    public class PostsByTime : AbstractIndexCreationTask<Post>
    {
        public PostsByTime()
        {
            Map = posts => from post in posts
                           where post.Published != null
                           select new {post.Published};
        }
    }
}