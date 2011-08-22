using System.Collections.Generic;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Models;
using EyePatch.Blog.Models.Forms;
using EyePatch.Blog.Models.Widgets;
using EyePatch.Core.Documents;

namespace EyePatch.Blog
{
    public interface IBlogManager
    {
        Page PostTemplate { get; }
        Page PostList { get; }
        Documents.Blog Settings { get; }

        IEnumerable<Post> All();
        BlogWindow BlogPanel();

        IEnumerable<Post> Drafts();
        IEnumerable<Post> Published();
        Post Load(string postId);
        Post Match(string url);

        Post Create(string title);
        void Update(PostForm form);
        void Rename(string id, string name);
        void Delete(string postId);

        void UpdateSettings(string listPage, string templateId, string disqus);
        void UpdateBody(string postId, string html);
        void Publish(string postId);

        // widget methods
        IEnumerable<TagCloudItem> TagCloud(int max);

        IEnumerable<Post> Posts(int page, int pageSize);
        IEnumerable<Post> Tagged(string tag, int page, int pageSize);
    }
}