using EyePatch.Blog.Entity;
using StructureMap.Configuration.DSL;

namespace EyePatch.Blog
{
    public class BlogRegistry : Registry
    {
        public BlogRegistry()
        {
            For<IBlogManager>().Use<BlogManager>();
            For<EyePatchBlogDataContext>().HybridHttpOrThreadLocalScoped().Use(() => new EyePatchBlogDataContext());
        }
    }
}