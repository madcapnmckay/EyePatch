using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using EyePatch.Core.Documents.Extensions;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace EyePatch.Core.Services
{
    public class PageService : ServiceBase, IPageService
    {
        private const string outputCacheKey = "AllEyepatchPagesOutputCache";
        protected ICacheProvider cacheProvider;

        protected IFolderService folderService;
        protected ITemplateService templateService;

        public PageService(IDocumentSession session, IFolderService folderService, ITemplateService templateService,
                           ICacheProvider cacheProvider)
            : base(session)
        {
            this.folderService = folderService;
            this.templateService = templateService;
            this.cacheProvider = cacheProvider;
        }

        #region IPageService Members

        public ResourceCollection Js(string id)
        {
            var page = Load(id);
            var distinctWidgets =
                page.ContentAreas.SelectMany(c => c.Widgets).GroupBy(w => w.Type).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.Js != null))
            {
                result.AddRange(widget.Js);
            }
            return result;
        }

        public ResourceCollection Css(string id)
        {
            var page = Load(id);
            var distinctWidgets =
                page.ContentAreas.SelectMany(w => w.Widgets).GroupBy(w => w.Type).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.Css != null))
            {
                result.AddRange(widget.Css);
            }
            return result;
        }

        public ResourceCollection AdminJs(string id)
        {
            var page = Load(id);
            var distinctWidgets =
                page.ContentAreas.SelectMany(w => w.Widgets).GroupBy(w => w.Type).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.AdminJs != null))
            {
                result.AddRange(widget.AdminJs);
            }
            return result;
        }

        public ResourceCollection AdminCss(string id)
        {
            var page = Load(id);
            var distinctWidgets =
                page.ContentAreas.SelectMany(w => w.Widgets).GroupBy(w => w.Type).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.AdminCss != null))
            {
                result.AddRange(widget.AdminCss);
            }
            return result;
        }

        protected void ChangeTemplate(Page page, string templateId)
        {
            var template = templateService.Load(templateId);

            page.TemplateId = template.Id;
            page.ContentAreas.Clear();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        public void ChangeTemplate(string pageID, string templateId)
        {
            var page = Load(pageID);
            ChangeTemplate(page, templateId);
        }

        public void AddOutputCacheDependency(HttpContext context)
        {
            if (context.Cache[outputCacheKey] == null)
            {
                context.Cache[outputCacheKey] = DateTime.UtcNow.ToString();
            }
            context.Response.AddCacheItemDependency(outputCacheKey);
        }

        public void ClearOutputCacheDependency(HttpContext context)
        {
            context.Cache.Remove(outputCacheKey);
        }

        public IEnumerable<Page> VisiblePages()
        {
            return
                session.Query<Page>().Where(p => !p.IsDynamic && !p.IsHidden && p.IsLive).OrderByDescending(
                    p => p.Priority).Take(1024);
        }

        public void Hide(Page page)
        {
            page.IsHidden = true;
            session.SaveChanges();
        }

        public int Count
        {
            get
            {
                try
                {
                    RavenQueryStatistics stats;
                    var results = session.Query<Page>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .Statistics(out stats)
                        .Where(x => x.Url != null && x.Url != string.Empty && !x.IsHidden)
                        .ToArray();

                    return stats.TotalResults;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public Page Match(string url)
        {
            return session.Query<Page>("PagesByUrl").Customize(x => x.WaitForNonStaleResultsAsOfNow()).Where(p => p.Url == url.NormalizeUrl()).SingleOrDefault();
        }

        public Page Load(string id)
        {
            var page = session.Load<Page>(id);
            if (page == null)
                throw new ApplicationException("Page does not exist");

            return page;
        }

        public IEnumerable<PageItem> LoadFromFolder(string folderID)
        {
            var folder = folderService.FindFolder(folderID);
            return folder.Pages.Where(p => p.IsLive && p.IsInMenu).OrderBy(p => p.MenuOrder);
        }

        public Page Homepage()
        {
            return Match("/");
        }

        public Page Create(string name, string title, string url, bool isLive, bool hidden)
        {
            return Create(name, title, url, folderService.RootFolder.Id, isLive, hidden);
        }

        public Page Create(string name, string title, string url, string folderId, bool isLive, bool hidden)
        {
            return Create(name, title, url, folderId, isLive, templateService.DefaultTemplate.Id, hidden);
        }

        public Page Create(string name, string title, string url, string folderId, bool isLive, string templateId, bool hidden)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var template = templateService.Load(templateId);
            var folder = folderService.FindFolder(folderId);

            if (folder == null)
                throw new ApplicationException("folder does not exist");

            var page = new Page
            {
                Name = name,
                Title = title,
                Url = url.NormalizeUrl(),
                IsLive = isLive,
                TemplateId = template.Id,
                // these should be configurable
                Priority = url == "/" ? 1 : 0.8,
                ChangeFrequency = url == "/" ? ChangeFrequency.Daily : ChangeFrequency.Monthly,
                AnalyticsKey = template.AnalyticsKey,
                Created = DateTime.UtcNow,
                IsHidden = hidden
            };

            session.Store(page);
            session.SaveChanges();

            // update folder with page id
            if (!hidden)
                folder.Pages.Add(new PageItem { Id = page.Id, Name = page.Name, Url = page.Url, IsHomePage = page.IsHomePage() });
            
            session.SaveChanges();

            ClearOutputCacheDependency(HttpContext.Current);
            return page;
        }

        public ContentArea CreateContentArea(Page page, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var contentArea = new ContentArea {Name = name};
            page.ContentAreas.Add(contentArea);
            session.SaveChanges();
            return contentArea;
        }

        public void Update(PageForm form)
        {
            var existingPage = Match(form.Url);
            if (existingPage != null && existingPage.Id != form.Id)
                throw new ApplicationException("A page with this url already exists");
            
            var page = Load(form.Id);

            PageItem pageItem;
            folderService.FindParentFolderOfPage(page.Id, out pageItem);

            if (page.Url == "/" && page.Url != form.Url.NormalizeUrl())
                throw new ApplicationException("Can't change the url of the homepage");


            page.Title = pageItem.Title = form.Title;
            page.Url = pageItem.Url = form.Url == null ? null : form.Url.NormalizeUrl();

            if (page.TemplateId != form.TemplateID)
            {
                ChangeTemplate(page, form.TemplateID);
            }

            page.IsLive = pageItem.IsLive = form.IsLive;
            page.IsInMenu = pageItem.IsInMenu = form.IsInMenu;
            page.MenuOrder = pageItem.MenuOrder =form.MenuOrder;
            page.LastModified = DateTime.UtcNow;

            session.SaveChanges();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Update(SearchForm form)
        {
            var page = Load(form.Id);

            PageItem pageItem;
            folderService.FindParentFolderOfPage(page.Id, out pageItem);

            page.Description = pageItem.Description = form.Description;
            page.Keywords = form.Keywords;
            page.Language = form.Language;
            page.Charset = form.Charset;
            page.Author = form.Author;
            page.Copyright = form.Copyright;
            page.Robots = form.Robots;
            page.LastModified = DateTime.UtcNow;

            session.SaveChanges();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Update(FacebookForm form)
        {
            var page = Load(form.Id);

            page.OgType = form.Type;
            page.OgEmail = form.Email;
            page.OgPhone = form.Phone;
            page.OgImage = form.Image;
            page.OgStreetAddress = form.StreetAddress;
            page.OgLocality = form.Locality;
            page.OgRegion = form.Region;
            page.OgCountry = form.Country;
            page.OgPostcode = form.Postcode;
            page.OgLongitude = form.Longitude;
            page.OgLatitude = form.Latitude;
            page.LastModified = DateTime.UtcNow;

            session.SaveChanges();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Rename(string id, string name)
        {
            var page = Load(id);
            page.Name = name;

            PageItem pageItem = null;
            folderService.FindParentFolderOfPage(page.Id, out pageItem);

            if (pageItem != null)
            {
                pageItem.Name = name;
            } 
            page.LastModified = DateTime.UtcNow;

            session.SaveChanges();
        }

        public void Move(string id, string parent)
        {
            PageItem pageItem;
            var oldParent = folderService.FindParentFolderOfPage(id, out pageItem);
            var newParent = folderService.FindFolder(parent);

            oldParent.Pages.Remove(pageItem);
            newParent.Pages.Add(pageItem);

            session.SaveChanges();
        }

        public void Delete(string id)
        {
            var page = session.Load<Page>(id);
            if (page == null)
                return; // nothing to do

            session.Delete(page);
            session.SaveChanges();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        #endregion
    }
}