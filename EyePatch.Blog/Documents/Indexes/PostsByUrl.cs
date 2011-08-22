using System.Linq;
using Raven.Client.Indexes;

namespace EyePatch.Blog.Documents.Indexes
{
    public class PostsByUrl : AbstractIndexCreationTask<Post>
    {
        public PostsByUrl()
        {
            Map = posts => from post in posts
                           select new { post.Url };
        }
    }
}