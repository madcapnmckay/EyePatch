using EyePatch.Core.Util;

namespace EyePatch.Blog.Documents
{
    public class Tag
    {
        public string Value { get; set; }
        public string Slug { get; set; }

        public Tag()
        {
            
        }

        public Tag(string tag)
        {
            Value = tag;
            Slug = tag.AsSlug();
        }
    }
}