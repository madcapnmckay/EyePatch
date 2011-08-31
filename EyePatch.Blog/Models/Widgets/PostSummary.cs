using System;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Documents.Extensions;
using EyePatch.Blog.Util.ActionResult;
using EyePatch.Blog.Util.Extensions;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Blog.Models.Widgets
{
    public class PostSummary : IRss
    {
        protected Documents.Blog blog;
        protected int index;
        protected int total;
        protected Post post;

        public PostSummary(Post post, Documents.Blog blog, int index, int total)
        {
            this.post = post;
            this.blog = blog;
            this.index = index + 1; // not zero indexed from users perspective
            this.total = total;
        }

        public bool IsFirst
        {
            get { return index == 0; }
        }

        public bool IsLast
        {
            get { return index == total; }
        }

        public bool IsEven
        {
            get { return index % 2 == 0; }
        }

        public string CssClass
        {
            get
            {
                var cssClass = "post-summary";

                if (IsFirst)
                    cssClass += " post-first";

                if (IsLast)
                    cssClass += " post-last";

                if (IsEven)
                    cssClass += " post-even";

                return cssClass;
            }
        }

        public bool CommentsEnabled
        {
            get { return blog.CommentsEnabled(); }
        }

        public string CommentCountUrl
        {
            get { return string.Format("{0}#disqus_thread", Permalink); }
        }

        public string Permalink
        {
            get { return post.Url.ToLowerInvariant().ToFullyQualifiedUrl(); }
        }

        public string DisqusID
        {
            get { return post.Id; }
        }

        public DateTime Published
        {
            get { return post.Published.HasValue ? post.Published.Value : DateTime.MinValue; }
        }

        public string PublishedTime
        {
            get { return string.Format("{0:HH:mm}", Published); }
        }

        public string PublishedDay
        {
            get { return string.Format("{0:dd}", Published); }
        }

        public string PublishedMonthShort
        {
            get { return string.Format("{0:MMM}", Published); }
        }

        public string PublishedYear
        {
            get { return string.Format("{0:yyyy}", Published); }
        }

        #region IRss Members

        public string Title
        {
            get { return post.Title; }
        }

        public string Description
        {
            get { return post.Body.TruncateWords(150); }
        }

        public string Link
        {
            get { return post.Url; }
        }

        #endregion
    }
}