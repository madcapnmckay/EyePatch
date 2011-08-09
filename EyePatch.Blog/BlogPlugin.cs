using System;
using System.Collections.Generic;
using System.Reflection;
using EyePatch.Blog.Widgets;
using EyePatch.Core;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Plugins;
using EyePatch.Core.Util;
using NKnockoutUI.Window;

namespace EyePatch.Blog
{
    public class BlogPlugin : IEyePatchPlugin
    {
        protected IContentManager contentManager;
        protected IBlogManager blogManager;

        public BlogPlugin(IContentManager contentManager, IBlogManager blogManager)
        {
            this.contentManager = contentManager;
            this.blogManager = blogManager;
        }

        public string Name
        {
            get { return "Blog"; }
        }

        public IList<Type> Widgets
        {
            get { return new List<Type> { typeof(PostList), typeof(TagCloud), typeof(PostBody) }; }
        }

        public IList<Window> Windows
        {
            get
            {
                var windows = new List<Window> { blogManager.BlogPanel() };
                return windows;
            }
        }

        public ResourceCollection Js
        {
            get { 
                var js = new ResourceCollection
                             {
                                 new EmbeddedResource("/js/eyepatch-blog-info.js", "EyePatch.Blog", Assembly.GetAssembly(GetType())),
                                 new EmbeddedResource("/js/eyepatch-blog.js", "EyePatch.Blog", Assembly.GetAssembly(GetType()))
                             };
                return js;
            }
        }

        public ResourceCollection Css
        {
            get
            {
                var css = new ResourceCollection { new EmbeddedResource("/css/eyepatch-blog.css", "EyePatch.Blog", Assembly.GetAssembly(GetType())) };
                return css;
            }
        }

        public string Author
        {
            get { return "Ian Mckay"; }
        }

        public void Register()
        {
            if (!DataSourceTools.TableExists("Post"))
            {
                DataSourceTools.CreateDatabaseTables(EmbeddedResourceTools.FileContents("EyePatch.Blog.SQL.InstallEyepatchBlog.sql", GetType().Assembly));

                // create the blog post page
                var postTemplate = contentManager.Page.Create("BlogPostTemplate", "EyePatch Blog Template", "/blog/template", false);
                // so it will not appear in the page list
                postTemplate.IsHidden = true;
                contentManager.Page.Update();
                // store for later
                blogManager.AssignPostTemplate(postTemplate.ID);
            }
        }

        public void Startup()
        {
            // nothing to do
            return;
        }

        public IEnumerable<ISiteMapItem> SiteMapItems()
        {
            return blogManager.Published();
        }
    }
}