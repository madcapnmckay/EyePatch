namespace EyePatch.Blog.Entity
{
    partial class BlogInfo
    {
        public bool CommentsEnabled
        {
            get { return !string.IsNullOrWhiteSpace(Disqus); }
        }
    }
}