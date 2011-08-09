using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EyePatch.Core.Entity;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Models.Tree;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Extensions;
using NKnockoutUI.Tabs;
using NKnockoutUI.Window;

namespace EyePatch.Core.Models
{
    public class AdminPanelViewModel
    {
        private static ResourceCollection js = null;
        private static ResourceCollection css = null;
        private static UrlHelper urlHelper;

        protected static UrlHelper Url
        {
            get
            {
                if (urlHelper == null)
                {
                    var requestContext = new System.Web.Routing.RequestContext(
                                    new HttpContextWrapper(HttpContext.Current),
                                    new System.Web.Routing.RouteData());
                    urlHelper = new System.Web.Mvc.UrlHelper(requestContext);
                }
                return urlHelper;
            }
        }

        public PageForm Page { get; set; }
        public List<Window> Windows { get; protected set; }
        public List<ResourcePath> Css { get; set; }
        public List<ResourcePath> Scripts { get; set; }
        public List<KeyValuePair<int, string>> TemplateList { get; set; }
        public PageTree Pages { get; set; }
        public WidgetTree Widgets { get; set; }
        public TemplateTree Templates { get; set; }
        public MediaTree MediaFolders { get; set; }
        public TabGroup Tabs { get; set; }
        public AdminRoutes Urls { get; set; }

        public bool Debug { get { return EyePatchApplication.ReleaseMode != ReleaseMode.Production; } }

        public AdminPanelViewModel(PageForm page)
        {
            Page = page;
            Windows = new List<Window>();
            Css = new List<ResourcePath>();
            Urls = new AdminRoutes();

            Windows.Add(new AdminWindow());
            Tabs = new TabGroup("EyePatchTabs");
            Tabs.Add(new Tab { Name = "Pages", CreateFunction = "ep.createPagesTab", IconCssClass = "pages", BodyCssClass = "pages-tab", IsActive = true });
            Tabs.Add(new Tab { Name = "Templates", CreateFunction = "ep.createTemplatesTab", IconCssClass = "templates", BodyCssClass = "templates-tab" });
            Tabs.Add(new Tab { Name = "Widgets", CreateFunction = "ep.createWidgetsTab", IconCssClass = "widgets", BodyCssClass = "widgets-tab" });
            Tabs.Add(new Tab { Name = "Images", CreateFunction = "ep.createImagesTab", IconCssClass = "images", BodyCssClass = "images-tab" });
        }

        public static ResourceCollection DependentJs
        {
            get
            {
                if (js == null)
                {
                    js = new ResourceCollection();
                    js.Load(Resources.jQueryUI)
                        .Load(Resources.jQueryValidate)
                        .Load(Resources.jQueryValidateUnobtrusive)
                        .Load(Resources.jQueryForm)
                        .Load(Resources.json2)
                        .Load(Resources.jQueryTmpl)
                        .Load(Resources.Knockout)
                        .Load(Resources.KnockoutMapping)
                        .Load(Resources.jQueryCookie)
                        .Load("/core/js/jquery.tiptip.js", MatchMode.FileName)
                        .Load("/core/js/splitter.js", MatchMode.Path)
                        .Load("/core/js/jquery.notice.js", MatchMode.FileName)
                        .Load("/core/js/ajaxfileupload.js", MatchMode.Path)
                        .Load("/core/js/Knockout-UI/ui-util.js", MatchMode.Path)
                        .Load("/core/js/Knockout-UI/ui-window.js", MatchMode.Path)
                        .Load("/core/js/Knockout-UI/ui-tabs.js", MatchMode.Path)
                        .Load("/core/js/Knockout-UI/ui-tree.js", MatchMode.Path)
                        .Load("/core/js/Knockout-UI/ui-contextmenu.js", MatchMode.Path)
                        .Load("/core/js/eyepatch-admin.js", MatchMode.Path);
                }
                return js;
            }
        }

        public static ResourceCollection DependentCss
        {
            get
            {
                if (css == null)
                {
                    css = new ResourceCollection();
                    css.Load(Url.ActionSeo("Css", "Admin"), MatchMode.Path, "text/css");
                }
                return css;
            }
        }
    }
}