using System.IO;
using System.Linq;
using System.Web;
using EyePatch.Core.Models;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Services;

namespace EyePatch.Core
{
    /// <summary>
    ///   An aggregator service class that provides the overall interface to the 
    ///   content management system
    /// </summary>
    public class ContentManager : IContentManager
    {
        public const string PluginDir = "~/plugins";
        public const string PluginTempDir = "~/plugins/temp";

        public ContentManager(IApplicationService applicationService, IPageService pageService,
                              IFolderService folderService, ITemplateService templateService,
                              IWidgetService widgetService, IResourceService resourceService,
                              IPluginService pluginService, IMediaService mediaService)
        {
            Application = applicationService;
            Page = pageService;
            Folder = folderService;
            Template = templateService;
            Widget = widgetService;
            Resources = resourceService;
            Plugin = pluginService;
            Media = mediaService;
        }

        #region IContentManager Members

        public IPageService Page { get; protected set; }
        public IFolderService Folder { get; protected set; }
        public ITemplateService Template { get; protected set; }
        public IWidgetService Widget { get; protected set; }
        public IApplicationService Application { get; protected set; }
        public IResourceService Resources { get; protected set; }
        public IPluginService Plugin { get; protected set; }
        public IMediaService Media { get; protected set; }

        public string SiteName
        {
            get { return Folder.RootFolder.Name; }
        }

        public string SiteDescription
        {
            get
            {
                var description = string.Empty;
                if (Template.DefaultTemplate != null && !string.IsNullOrWhiteSpace(Template.DefaultTemplate.Description))
                    description = Template.DefaultTemplate.Description;

                return description;
            }
        }

        /// <summary>
        ///   The core of the whole system, this method returns the viewModel consumed by the admin panel
        ///   javascript interface
        /// </summary>
        /// <returns></returns>
        public AdminPanelViewModel AdminPanel(string pageId)
        {
            var page = Page.Load(pageId);

            var viewModel = new AdminPanelViewModel(page.ToViewModel());
            viewModel.Pages = Folder.RootFolder.ToViewModel();
            viewModel.Widgets = Plugin.All().ToViewModel();

            var templates = Template.All();
            viewModel.Templates = templates.ToViewModel();
            viewModel.TemplateList = templates.ToDictionary(k => k.Id, v => v.Name).ToList();

            var mediaFolder = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Media/"));
            if (!mediaFolder.Exists)
                mediaFolder.Create();

            viewModel.MediaFolders = mediaFolder.ToViewModel();

            // scripts for the eyepatch admin window
            var scripts = AdminPanelViewModel.DependentJs;
            // scripts for any plugins in the UI
            scripts.AddRange(Plugin.UserInterfaceJs());
            // scripts for the widgets on the page
            scripts.AddRange(Page.AdminJs(page.Id));
            // css for the eyepatch admin widnow
            var css = AdminPanelViewModel.DependentCss;
            // css for any plugins in the UI
            css.AddRange(Plugin.UserInterfaceCss());
            // css for the widgets on the page
            css.AddRange(Page.AdminCss(page.Id));

            viewModel.Scripts =
                Resources.GetResourcePaths(scripts.Except(Page.Js(page.Id)).ToResourceCollection()).ToList();
            viewModel.Css = Resources.GetResourcePaths(css.Except(Page.Css(page.Id)).ToResourceCollection()).ToList();

            // inspect widgets for their own windows and add to viewmodel
            viewModel.Windows.AddRange(Plugin.UserInterfaceWindows());

            return viewModel;
        }

        #endregion
    }
}