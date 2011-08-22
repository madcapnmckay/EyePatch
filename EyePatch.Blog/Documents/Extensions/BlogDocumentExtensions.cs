namespace EyePatch.Blog.Documents.Extensions
{
    public static class BlogDocumentExtensions
    {
        public static bool CommentsEnabled(this Blog blog)
        {
            return !string.IsNullOrWhiteSpace(blog.DisqusShortName);
        }
    }
}