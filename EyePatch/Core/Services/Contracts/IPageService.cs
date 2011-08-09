using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using EyePatch.Core.Entity;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Services
{
    public interface IPageService
    {
        int Count { get; }
        ConcurrentDictionary<string, Page> All();

        Page Match(string url);
        Page Load(int id);
        IEnumerable<Page> LoadFromFolder(Folder folder);
        Page Homepage();
        
        // CRUD
        Page Create(string name, string title, string url, bool isLive);
        Page Create(string name, string title, string url, int folderID, bool isLive);
        Page Create(string name, string title, string url, bool isLive, int templateID, int folderID);

        ContentArea CreateContentArea(Page page, string name);
        void DeleteContentArea(ContentArea contentArea);

        void Update(PageForm form);
        void Update(SearchForm form);
        void Update(FacebookForm form);
        void Update();
        void Rename(int id, string name);
        void Move(int id, int parent);
        void Delete(int id);

        void InvalidatePageCache();
        /// <summary>
        /// Returns all the javascript files referenced on that page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResourceCollection Js(int id);
        /// <summary>
        /// Returns all the css files referenced on that page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResourceCollection Css(int id);
        /// <summary>
        /// Returns all the javascript files need to display the admin interface for that page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResourceCollection AdminJs(int id);
        /// <summary>
        /// Returns all the css files need to display the admin interface for that page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResourceCollection AdminCss(int id);
        /// <summary>
        /// When a page template changes all the existing widgets must be deleted
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="templateId"></param>
        void ChangeTemplate(int pageID, int templateId);

        /// <summary>
        /// Adds a cache dependency property for the page
        /// </summary>
        void AddOutputCacheDependency(HttpContext context);

        /// <summary>
        /// Removes the global dependency, clearing the output cache for all pages
        /// </summary>
        /// <param name="context"></param>
        void ClearOutputCacheDependency(HttpContext context);
    }
}