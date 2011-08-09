using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using EyePatch.Core.Entity;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Mvc.Resources;
using System.Collections.Concurrent;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Core.Services
{
    public class PageService : ServiceBase, IPageService
    {
        private static object padlock = new object();
        private const string pageListKey = "AllEyepatchPages";
        private const string outputCacheKey = "AllEyepatchPagesOutputCache";

        protected IFolderService folderService;
        protected ITemplateService templateService;
        protected ICacheProvider cacheProvider;

        public PageService(EyePatchDataContext context, IFolderService folderService, ITemplateService templateService, ICacheProvider cacheProvider)
            : base(context)
        {
            this.folderService = folderService;
            this.templateService = templateService;
            this.cacheProvider = cacheProvider;

            if (db.LoadOptions == null)
            {
                var dlo = new DataLoadOptions();
                dlo.LoadWith<Page>(p => p.Template);
                dlo.LoadWith<Page>(p => p.ContentAreas);
                db.LoadOptions = dlo;
            }
        }

        #region IPageService Members

        public void InvalidatePageCache()
        {
            lock (PadLock)
            {
                cacheProvider.Remove(pageListKey);
            }
        }

        public ResourceCollection Js(int id)
        {
            var page = Load(id);
            var distinctWidgets = page.ContentAreas.SelectMany(w => w.WidgetInstances).GroupBy(w => w.WidgetID).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.Js != null))
            {
                result.AddRange(widget.Js);
            }
            return result;
        }

        public ResourceCollection Css(int id)
        {
            var page = Load(id);
            var distinctWidgets = page.ContentAreas.SelectMany(w => w.WidgetInstances).GroupBy(w => w.WidgetID).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.Css != null))
            {
                result.AddRange(widget.Css);
            }
            return result;
        }

        public ResourceCollection AdminJs(int id)
        {
            var page = Load(id);
            var distinctWidgets = page.ContentAreas.SelectMany(w => w.WidgetInstances).GroupBy(w => w.WidgetID).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.AdminJs != null))
            {
                result.AddRange(widget.AdminJs);
            }
            return result;
        }

        public ResourceCollection AdminCss(int id)
        {
            var page = Load(id);
            var distinctWidgets = page.ContentAreas.SelectMany(w => w.WidgetInstances).GroupBy(w => w.WidgetID).Select(g => g.First().GetInstance());
            var result = new ResourceCollection();
            foreach (var widget in distinctWidgets.Where(w => w.AdminCss != null))
            {
                result.AddRange(widget.AdminCss);
            }
            return result;
        }

        public void ChangeTemplate(int pageID, int templateId)
        {
            var page = Load(pageID);
            var template = templateService.Load(templateId);

            db.WidgetInstances.DeleteAllOnSubmit(page.ContentAreas.SelectMany(c => c.WidgetInstances));
            db.ContentAreas.DeleteAllOnSubmit(page.ContentAreas);

            InvalidatePageCache();
            ClearOutputCacheDependency(HttpContext.Current);

            page.Template = template;
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

        public int Count
        {
            get
            {
                try
                {
                    return All().Count;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public ConcurrentDictionary<string, Page> All()
        {
            var pages = cacheProvider.Get<ConcurrentDictionary<string, Page>>(pageListKey);
            if (pages == null)
            {
                lock (padlock)
                {
                    pages = cacheProvider.Get<ConcurrentDictionary<string, Page>>(pageListKey);
                    if (pages == null)
                    {
                        pages = new ConcurrentDictionary<string, Page>(db.Pages.Where(p => p.Url != null && p.Url.Trim() != string.Empty && !p.IsHidden).Select(p => new KeyValuePair<string, Page>(p.Url, p)));
                        cacheProvider.Add(pageListKey, pages);
                    }
                }
            }
            return pages;
        }

        public Page Match(string url)
        {
            Page result;
            All().TryGetValue(url.NormalizeUrl(), out result);
            return result;
        }

        public Page Load(int id)
        {
            Page page = db.Pages.SingleOrDefault(p => p.ID == id);
            if (page == null)
                throw new ApplicationException("Page does not exist");

            return page;
        }

        public IEnumerable<Page> LoadFromFolder(Folder folder)
        {
            return All().Where(p => p.Value.FolderID == folder.ID && p.Value.IsLive && p.Value.IsInMenu).Select(p => p.Value).OrderBy(p => p.MenuOrder);
        }

        public Page Homepage()
        {
            return Match("/");
        }

        public Page Create(string name, string title, string url, bool isLive)
        {
            return Create(name, title, url, isLive, templateService.DefaultTemplate.ID, 1);
        }

        public Page Create(string name, string title, string url, int folderID, bool isLive)
        {
            return Create(name, title, url, isLive, templateService.DefaultTemplate.ID, folderID);
        }

        public Page Create(string name, string title, string url, bool isLive, int templateID, int folderID)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Folder folder = folderService.Load(folderID);
            Template template = templateService.Load(templateID);

            var page = new Page
                           {
                               Name = name,
                               Title = title,
                               Url = url.NormalizeUrl(),
                               IsLive = isLive,
                               Template = template,
                               Folder = folder,
                               // these should be configurable
                               Priority = url == "/" ? 1 : 0.8, 
                               ChangeFrequency = url == "/" ? ChangeFrequency.Daily : ChangeFrequency.Monthly
                           };
            db.Pages.InsertOnSubmit(page);
            db.SubmitChanges();
            InvalidatePageCache();
            ClearOutputCacheDependency(HttpContext.Current);
            return page;
        }

        public ContentArea CreateContentArea(Page page, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            
            var contentArea = new ContentArea { Name = name };
            page.ContentAreas.Add(contentArea);
            db.SubmitChanges();
            return contentArea;
        }

        public void DeleteContentArea(ContentArea contentArea)
        {
            db.WidgetInstances.DeleteAllOnSubmit(contentArea.WidgetInstances);
            db.ContentAreas.DeleteOnSubmit(contentArea);
            db.SubmitChanges();
        }

        public void Update(PageForm form)
        {
            var existingPage = Match(form.Url);
            if (existingPage != null && existingPage.ID != form.Id)
                throw new ApplicationException("A page with this url already exists");

            var page = Load(form.Id);
            page.Title = form.Title;

            if (page.Url == "/" && page.Url != form.Url.NormalizeUrl() )
                throw new ApplicationException("Can't change the url of the homepage");

            page.Url = form.Url == null ? page.Url : form.Url.NormalizeUrl();
            page.Template = templateService.Load(form.TemplateID);
            page.IsLive = form.IsLive;
            page.IsInMenu = form.IsInMenu;
            page.MenuOrder = form.MenuOrder;
            db.SubmitChanges();
            InvalidatePageCache();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Update(SearchForm form)
        {
            var page = Load(form.Id);

            page.Description = form.Description;
            page.Keywords = form.Keywords;
            page.Language = form.Language;
            page.Charset = form.Charset;
            page.Author = form.Author;
            page.Copyright = form.Copyright;
            page.Robots = form.Robots;

            db.SubmitChanges();
            InvalidatePageCache();
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

            db.SubmitChanges();
            InvalidatePageCache();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Update()
        {
            db.SubmitChanges();
        }

        public void Rename(int id, string name)
        {
            Page page = Load(id);
            page.Name = name;
            db.SubmitChanges();
        }

        public void Move(int id, int parent)
        {
            Page page = Load(id);
            Folder newParent = folderService.Load(parent);

            page.Folder = newParent;
            db.SubmitChanges();
        }

        public void Delete(int id)
        {
            Page page = Load(id);
            db.WidgetInstances.DeleteAllOnSubmit(page.ContentAreas.SelectMany(c => c.WidgetInstances));
            db.ContentAreas.DeleteAllOnSubmit(page.ContentAreas);
            db.Pages.DeleteOnSubmit(page);
            db.SubmitChanges();
            InvalidatePageCache();
            ClearOutputCacheDependency(HttpContext.Current);
        }

        #endregion
    }
}