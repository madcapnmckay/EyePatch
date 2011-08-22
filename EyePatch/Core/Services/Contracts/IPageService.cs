using System.Collections.Generic;
using System.Web;
using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Services
{
    public interface IPageService
    {
        int Count { get; }

        Page Match(string url);
        Page Load(string id);

        IEnumerable<PageItem> LoadFromFolder(string folderID);

        Page Homepage();

        // CRUD
        Page Create(string name, string title, string url, bool isLive, bool hidden = false);
        Page Create(string name, string title, string url, string folderId, bool isLive, bool hidden = false);
        Page Create(string name, string title, string url, string folderId, bool isLive, string templateId, bool hidden = false);

        ContentArea CreateContentArea(Page page, string name);

        void Update(PageForm form);
        void Update(SearchForm form);
        void Update(FacebookForm form);
        void Rename(string id, string name);
        void Move(string id, string parent);
        void Delete(string id);

        /// <summary>
        ///   Returns all the javascript files referenced on that page
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        ResourceCollection Js(string id);

        /// <summary>
        ///   Returns all the css files referenced on that page
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        ResourceCollection Css(string id);

        /// <summary>
        ///   Returns all the javascript files need to display the admin interface for that page
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        ResourceCollection AdminJs(string id);

        /// <summary>
        ///   Returns all the css files need to display the admin interface for that page
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        ResourceCollection AdminCss(string id);

        /// <summary>
        ///   When a page template changes all the existing widgets must be deleted
        /// </summary>
        /// <param name = "pageId"></param>
        /// <param name = "templateId"></param>
        void ChangeTemplate(string pageId, string templateId);

        /// <summary>
        ///   Adds a cache dependency property for the page
        /// </summary>
        void AddOutputCacheDependency(HttpContext context);

        /// <summary>
        ///   Removes the global dependency, clearing the output cache for all pages
        /// </summary>
        /// <param name = "context"></param>
        void ClearOutputCacheDependency(HttpContext context);

        /// <summary>
        ///   A list of all visible pages, used for sitemaps etc
        /// </summary>
        /// <returns></returns>
        IEnumerable<Page> VisiblePages();

        /// <summary>
        ///   Set a page as hidden
        /// </summary>
        /// <param name = "page"></param>
        void Hide(Page page);
    }
}