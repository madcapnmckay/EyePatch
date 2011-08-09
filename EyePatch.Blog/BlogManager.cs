using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EyePatch.Blog.Entity;
using EyePatch.Blog.Models;
using EyePatch.Blog.Models.Forms;
using EyePatch.Blog.Models.Widgets;
using EyePatch.Core.Entity;
using EyePatch.Core.Services;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Blog
{
    public class BlogManager : ServiceBase, IBlogManager
    {
        private static object padlock = new object();
        protected readonly EyePatchBlogDataContext blogDb;
        protected readonly IPageService pageService;
        protected readonly ICacheProvider cacheProvider;
        private const string postListKey = "AllEyepatchBlogPosts";
        private const string settingsKey = "EyePatchBlogSettings";

        public BlogManager(EyePatchDataContext context, EyePatchBlogDataContext blogContext, IPageService pageService, ICacheProvider cacheProvider)
            : base(context)
        {
            blogDb = blogContext;
            this.pageService = pageService;
            this.cacheProvider = cacheProvider;
        }

        public void InvalidatePostCache()
        {
            lock (PadLock)
            {
                cacheProvider.Remove(postListKey);
            }
        }

        public Page Template
        {
            get
            {
                return pageService.Load(Settings.PostPageID);
            }
        }

        public Page PostList
        {
            get
            {
                return pageService.Load(Settings.ListPageID);
            }
        }

        public BlogInfo Settings
        {
            get
            {
                if (cacheProvider.Get<BlogInfo>(settingsKey) == null)
                {
                    cacheProvider.Add(settingsKey, blogDb.BlogInfos.First());
                }
                return cacheProvider.Get<BlogInfo>(settingsKey);
            }
        }

        public ConcurrentDictionary<string, Post> All()
        {
            var posts = cacheProvider.Get<ConcurrentDictionary<string, Post>>(postListKey);
            if (posts == null)
            {
                lock (padlock)
                {
                    posts = cacheProvider.Get<ConcurrentDictionary<string, Post>>(postListKey);
                    if (posts == null)
                    {
                        posts = new ConcurrentDictionary<string, Post>(blogDb.Posts.Where(p => p.Url != null && p.Url.Trim() != string.Empty).Select(p => new KeyValuePair<string, Post>(p.Url, p)));
                        cacheProvider.Add(postListKey, posts);
                    }
                }
            }
            return posts;
        }

        public BlogWindow BlogPanel()
        {
            var window = new BlogWindow();
            var blogPanelContents = new BlogWindowContents();

            blogPanelContents.Pages = pageService.All().Select(p => new KeyValuePair<int, string>(p.Value.ID, p.Value.Name));
            blogPanelContents.Drafts = Drafts().ToDraftsTree();
            blogPanelContents.Published = Published().ToPublishedTree();

            if (blogPanelContents.Drafts.Children.Any())
                blogPanelContents.Tabs.Members[1].IsActive = true;
            else
                blogPanelContents.Tabs.Members[0].IsActive = true;

            window.Contents = blogPanelContents;
            return window;
        }

        public IEnumerable<Post> Drafts()
        {
            return blogDb.Posts.Where(p => p.Published == null);
        }

        public IEnumerable<Post> Published()
        {
            return blogDb.Posts.Where(p => p.Published <= DateTime.UtcNow).OrderByDescending(p => p.Published);
        }

        public IEnumerable<Tag> TagContaining(string query)
        {
            return blogDb.Tags.Where(t => t.Name.Contains(query));
        }

        public Post Load(int id)
        {
            Post post = blogDb.Posts.SingleOrDefault(p => p.ID == id);
            if (post == null)
                throw new ApplicationException("Post does not exist");

            return post;
        }

        public Post Match(string path)
        {
            Post result;
            var all = All().ToList();
            var p = path.NormalizeUrl();

            All().TryGetValue(path.NormalizeUrl(), out result);
            return result;
        }

        public Post Create(string name)
        {
            var post = new Post();
            post.Title = string.Empty;
            post.Url = string.Empty;
            post.Body = Entity.Post.DefaultBody;
            post.Name = name;
            blogDb.Posts.InsertOnSubmit(post);
            blogDb.SubmitChanges();
            InvalidatePostCache();

            return post;
        }

        public void Update(PostForm form)
        {
            var existingPost = Match(form.Url);
            if (existingPost != null && existingPost.ID != form.Id)
                throw new ApplicationException("A post with this url already exists");

            var existingPage = pageService.Match(form.Url);
            if (existingPage != null)
                throw new ApplicationException("A page with this url already exists");

            var post = Load(form.Id);
            post.Title = form.Title;
            post.Url = form.Url == null ? post.Url : form.Url.NormalizeUrl();

            // parse tags
            if (!string.IsNullOrWhiteSpace(form.Tags))
            {
                var tagNames = form.Tags.Split(',').Select(t => t.Trim().ToLowerInvariant());
                foreach (var newTag in tagNames.Except(post.PostTags.Select(t => t.Tag.Name)))
                {
                    var name = newTag;
                    var tag = blogDb.Tags.SingleOrDefault(t => t.Name == name);
                    if (tag == null)
                    {
                        tag = new Tag { Name = name };
                        blogDb.Tags.InsertOnSubmit(tag);
                        post.PostTags.Add(new PostTag { Tag = tag });
                    }
                }
            }

            blogDb.SubmitChanges();
            InvalidatePostCache();
        }

        public void Rename(int id, string name)
        {
            Post post = Load(id);
            post.Name = name;
            blogDb.SubmitChanges();
        }

        public void Delete(int id)
        {
            Post post = Load(id);
            blogDb.PostTags.DeleteAllOnSubmit(post.PostTags);
            blogDb.Posts.DeleteOnSubmit(post);
            blogDb.SubmitChanges();
            InvalidatePostCache();
        }

        public void AssignPostTemplate(int id)
        {
            var info = blogDb.BlogInfos.FirstOrDefault();
            if (info == null)
            {
                info = new BlogInfo();
                blogDb.BlogInfos.InsertOnSubmit(info);
            }

            info.PostPageID = id;
            blogDb.SubmitChanges();
            cacheProvider.Remove(settingsKey);
        }

        public void UpdateSettings(int listPage, int templateId, string disqus)
        {
            var settings = blogDb.BlogInfos.First();

            settings.ListPageID = listPage;
            settings.Disqus = string.IsNullOrWhiteSpace(disqus) ? null : disqus;

            if (Template.TemplateID != templateId)
                pageService.ChangeTemplate(Template.ID, templateId);

            blogDb.SubmitChanges();
            cacheProvider.Remove(settingsKey);
        }

        public void Publish(int id)
        {
            var post = Load(id);
            post.Published = DateTime.UtcNow;
            blogDb.SubmitChanges();
            InvalidatePostCache();
        }

        public IList<KeyValuePair<Tag, int>> TagCloud(int max)
        {
            return Published().SelectMany(p => p.PostTags)
                .GroupBy(k => k.Tag).Select(t => new KeyValuePair<Tag, int>(t.Key, t.Count()))
                .OrderBy(t => t.Value)
                .ToList();
        }

        public IList<Post> Posts(int page, int pageSize)
        {
            return Published().Skip(page - 1).Take(pageSize).ToList();
        }

        public IList<Post> Tagged(string tag, int page, int pageSize)
        {
            return blogDb.Tags.Where(t => t.Name == tag)
                           .SelectMany(t => t.PostTags)
                           .Select(pt => pt.Post)
                           .Where(p => p.Published <= DateTime.UtcNow)
                           .OrderByDescending(p => p.Published)
                           .ToList();
        }
    }
}