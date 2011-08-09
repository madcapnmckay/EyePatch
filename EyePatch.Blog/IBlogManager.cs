using System.Collections.Concurrent;
using System.Collections.Generic;
using EyePatch.Blog.Entity;
using EyePatch.Blog.Models;
using EyePatch.Blog.Models.Forms;
using EyePatch.Core.Entity;

namespace EyePatch.Blog
{
    public interface IBlogManager
    {
        Page Template { get; }
        Page PostList { get; }
        BlogInfo Settings { get; }

        ConcurrentDictionary<string, Post> All();
        BlogWindow BlogPanel();

        IEnumerable<Post> Drafts();
        IEnumerable<Post> Published();
        IEnumerable<Tag> TagContaining(string query);
        Post Load(int id);
        Post Match(string path);

        Post Create(string title);
        void Update(PostForm form);
        void Rename(int id, string name);
        void Delete(int id);

        void AssignPostTemplate(int id);       
        void UpdateSettings(int listPage, int templateId, string disqus);
        void Publish(int id);   

        // widget methods
        IList<KeyValuePair<Tag, int>> TagCloud(int max);

        IList<Post> Posts(int page, int pageSize);
        IList<Post> Tagged(string tag, int page, int pageSize);
    }
}
