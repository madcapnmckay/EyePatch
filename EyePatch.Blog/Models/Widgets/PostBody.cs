using System;
using System.Collections.Generic;
using System.Linq;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Documents.Extensions;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Blog.Models.Widgets
{
    public class PostBody
    {
        protected Documents.Blog blog;
        protected Post post;

        public PostBody(Post post, Documents.Blog blog)
        {
            this.post = post;
            this.blog = blog;
        }

        public string PostId
        {
            get { return post.Id; }
        }

        public string Title
        {
            get { return post.Title; }
        }

        public string Body
        {
            get { return post.Body; }
        }

        public IList<Tag> Tags
        {
            get { return post.Tags.ToList(); }
        }

        public bool CommentsEnabled
        {
            get { return blog.CommentsEnabled(); }
        }

        public string DisqusShortName
        {
            get { return blog.DisqusShortName; }
        }

        public string DisqusID
        {
            get { return post.Id; }
        }

        public string CommentCountUrl
        {
            get { return string.Format("{0}#disqus_thread", Permalink); }
        }

        public string Permalink
        {
            get { return post.Url.ToLowerInvariant().ToFullyQualifiedUrl(); }
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
    }
}