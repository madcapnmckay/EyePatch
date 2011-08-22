using StructureMap.Configuration.DSL;

namespace EyePatch.Blog
{
    public class BlogRegistry : Registry
    {
        public BlogRegistry()
        {
            For<IBlogManager>().Use<BlogManager>();
        }
    }
}