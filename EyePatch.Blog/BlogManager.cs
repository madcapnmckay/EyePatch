using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EyePatch.Blog.Documents;
using EyePatch.Blog.Models;
using EyePatch.Blog.Models.Forms;
using EyePatch.Blog.Models.Widgets;
using EyePatch.Core;
using EyePatch.Core.Documents;
using EyePatch.Core.Services;
using EyePatch.Core.Util.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace EyePatch.Blog
{
    public class BlogManager : ServiceBase, IBlogManager
    {
        protected static object padLock = new object();
        protected Documents.Blog settings;
        protected readonly IContentManager contentManager;

        public BlogManager(IDocumentSession session, IContentManager contentManager)
            : base(session)
        {
            this.contentManager = contentManager;
        }

        #region IBlogManager Members

        public Documents.Blog Settings
        {
            get { 
                var settings = session.Query<Documents.Blog>().Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).FirstOrDefault();
                if (settings == null)
                {
                    lock (padLock)
                    {
                        settings = session.Query<Documents.Blog>().Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).FirstOrDefault();
                        if (settings == null)
                        {
                            // create the blog post page
                            var postTemplate = contentManager.Page.Create("BlogPostTemplate", "EyePatch Blog Template",
                                                                          "/blog/template", false);

                            // store for later
                            settings = new Documents.Blog();
                            settings.PostPageId = postTemplate.Id;
                            session.Store(settings);
                            session.SaveChanges();
                        }
                    }
                }
                return settings;
            }
        }

        public Page PostTemplate
        {
            get { return contentManager.Page.Load(Settings.PostPageId); }
        }

        public Page PostList
        {
            get { return contentManager.Page.Load(Settings.ListPageId); }
        }

        public IEnumerable<Post> All()
        {
            return session.Query<Post>().Where(p => !string.IsNullOrWhiteSpace(p.Url)).Take(1024);
        }

        public BlogWindow BlogPanel()
        {
            var window = new BlogWindow();
            var blogPanelContents = new BlogWindowContents();

            blogPanelContents.Pages =
                contentManager.Page.VisiblePages().Select(p => new KeyValuePair<string, string>(p.Id, p.Name));
            blogPanelContents.Drafts = Drafts().ToDraftsTree();
            blogPanelContents.Published = Published().ToPublishedTree();
            blogPanelContents.Disqus = Settings.DisqusShortName;
            blogPanelContents.Template = Settings.TemplateId;
            blogPanelContents.ListPage = Settings.ListPageId;

            if (blogPanelContents.Drafts.Children.Any())
                blogPanelContents.Tabs.Members[1].IsActive = true;
            else
                blogPanelContents.Tabs.Members[0].IsActive = true;

            window.Contents = blogPanelContents;
            return window;
        }

        public IEnumerable<Post> Drafts()
        {
            return session.Query<Post>().Where(p => p.Published == null);
        }

        public IEnumerable<Post> Published()
        {
            return session.Query<Post>().Where(p => p.Published <= DateTime.UtcNow).OrderByDescending(p => p.Published);
        }

        public Post Load(string postId)
        {
            var post = session.Load<Post>(postId);
            if (post == null)
                throw new ApplicationException("Post does not exist");

            return post;
        }

        public Post Match(string url)
        {
            var u = url.NormalizeUrl();
            return session.Query<Post>("PostsByUrl").Where(p => p.Url == url.NormalizeUrl()).SingleOrDefault();
        }

        public Post Create(string name)
        {
            var post = new Post {Title = string.Empty, Url = string.Empty, Name = name, Created = DateTime.UtcNow };

            session.Store(post);
            session.SaveChanges();
            return post;
        }

        public void Update(PostForm form)
        {
            var existingPost = Match(form.Url);
            if (existingPost != null && existingPost.Id != form.Id)
                throw new ApplicationException("A post with this url already exists");

            var existingPage = contentManager.Page.Match(form.Url);
            if (existingPage != null)
                throw new ApplicationException("A page with this url already exists");

            var post = Load(form.Id);
            post.Title = form.Title;
            post.Url = form.Url == null ? post.Url : form.Url.NormalizeUrl();

            post.Tags.Clear();

            if (!string.IsNullOrWhiteSpace(form.Tags))
            {
                post.Tags.AddRange(
                    form.Tags.Split(',').Where(t => t.Trim() != "").Select(
                        t => new Tag(t.Trim().ToLowerInvariant())));
            }

            post.LastModified = DateTime.UtcNow;

            session.SaveChanges();

            contentManager.Page.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Rename(string id, string name)
        {
            var post = Load(id);
            post.Name = name;
            post.LastModified = DateTime.UtcNow;
            session.SaveChanges();
        }

        public void Delete(string postId)
        {
            var post = Load(postId);
            session.Delete(post);
            session.SaveChanges();
        }

        public void UpdateSettings(string listPage, string templateId, string disqus)
        {
            Settings.ListPageId = listPage;
            Settings.DisqusShortName = string.IsNullOrWhiteSpace(disqus) ? null : disqus;
            Settings.TemplateId = templateId;

            if (PostTemplate.TemplateId != templateId)
                contentManager.Page.ChangeTemplate(PostTemplate.Id, templateId);

            session.SaveChanges();

            contentManager.Page.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void UpdateBody(string postId, string html)
        {
            var post = Load(postId);
            post.Body = html;
            session.SaveChanges();

            contentManager.Page.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Publish(string postId)
        {
            var post = Load(postId);
            post.Published = DateTime.UtcNow;
            session.SaveChanges();

            contentManager.Page.ClearOutputCacheDependency(HttpContext.Current);
        }

        public TagCloud TagCloud(int max)
        {
            RavenQueryStatistics stats;
            session.Query<Post>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Statistics(out stats)
                .Where(x => x.Published != null && x.Published <= DateTime.UtcNow)
                .ToArray();

            return new TagCloud(session.Query<TagCloudItem>("TagCloud").Take(max).ToList().OrderByDescending(t => t.Tag.Value), stats.TotalResults);
        }

        public IEnumerable<Post> Posts(int page, int pageSize, out int totalResults)
        {
            RavenQueryStatistics stats;
            var results = session.Query<Post>("PostsByTime").Statistics(out stats)
                .Where(p => p.Published < DateTime.UtcNow).OrderByDescending(p => p.Published).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            totalResults = stats.TotalResults;
            return results;
        }

        public IEnumerable<Post> Tagged(string tagSlug, int page, int pageSize, out int totalRecords)
        {
            RavenQueryStatistics stats;
            var results = session.Query<Post>("PostsByTag").Statistics(out stats)
                .Where(p => p.Tags.Any(t => t.Slug == tagSlug)).OrderByDescending(p => p.Published).Skip((page - 1) * pageSize).Take(pageSize);
            totalRecords = stats.TotalResults;
            return results;
        }

        #endregion
    }
}