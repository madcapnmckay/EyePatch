using System.Collections.Generic;
using NKnockoutUI.Tabs;

namespace EyePatch.Blog.Models
{
    public class BlogWindowContents
    {
        public BlogWindowContents()
        {
            Urls = new BlogRoutes();

            Tabs = new TabGroup("BlogTabs");
            Tabs.Add(new Tab
                         {
                             Name = "Blog",
                             CreateFunction = "ep.blog.createBlogTab",
                             IconCssClass = "blog",
                             BodyCssClass = "blog-tab"
                         });

            Tabs.Add(new Tab
                         {
                             Name = "Drafts",
                             CreateFunction = "ep.blog.createDraftsTab",
                             IconCssClass = "drafts",
                             BodyCssClass = "drafts-tab"
                         });
            Tabs.Add(new Tab
                         {
                             Name = "Published",
                             CreateFunction = "ep.blog.createPublishedTab",
                             IconCssClass = "published",
                             BodyCssClass = "published-tab"
                         });
        }

        public TabGroup Tabs { get; protected set; }

        public IEnumerable<KeyValuePair<string, string>> Pages { get; set; }

        public string Disqus { get; set; }

        public string ListPage { get; set; }

        public string Template { get; set; }

        public DraftsTree Drafts { get; set; }

        public PublishedTree Published { get; set; }

        public BlogRoutes Urls { get; set; }
    }
}