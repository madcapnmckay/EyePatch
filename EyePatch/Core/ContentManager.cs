using System;
using System.IO;
using System.Web;
using EyePatch.Core.Models;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Services;
using System.Linq;

namespace EyePatch.Core
{
    /// <summary>
    /// An aggregator service class that provides the overall interface to the 
    /// content management system
    /// </summary>
    public class ContentManager : IContentManager
    {
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

        public ContentManager(IApplicationService applicationService, IPageService pageService, IFolderService folderService, ITemplateService templateService, 
            IWidgetService widgetService, IResourceService resourceService, IPluginService pluginService, IMediaService mediaService)
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

        /// <summary>
        /// The core of the whole system, this method returns the viewModel consumed by the admin panel
        /// javascript interface
        /// </summary>
        /// <returns></returns>
        public AdminPanelViewModel AdminPanel(int pageId)
        {
            var page = Page.Load(pageId);
            var homepage = Page.Homepage();

            var viewModel = new AdminPanelViewModel(page.ToViewModel(homepage));
            viewModel.Pages = Folder.All().ToViewModel();
            viewModel.Widgets = Plugin.All().ToViewModel();
            viewModel.Templates = Template.All().ToViewModel();
            viewModel.TemplateList = Template.All().ToDictionary(k => k.ID, v => v.Name).ToList();

            viewModel.MediaFolders = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Media/")).ToViewModel();

            // scripts for the eyepatch admin window
            var scripts = AdminPanelViewModel.DependentJs;
            // scripts for any plugins in the UI
            scripts.AddRange(Plugin.UserInterfaceJs());
            // scripts for the widgets on the page
            scripts.AddRange(Page.AdminJs(page.ID));
            // css for the eyepatch admin widnow
            var css = AdminPanelViewModel.DependentCss;
            // css for any plugins in the UI
            css.AddRange(Plugin.UserInterfaceCss());
            // css for the widgets on the page
            css.AddRange(Page.AdminCss(page.ID));

            viewModel.Scripts = Resources.GetResourcePaths(scripts.Except(Page.Js(page.ID)).ToResourceCollection()).ToList();
            viewModel.Css = Resources.GetResourcePaths(css.Except(Page.Css(page.ID)).ToResourceCollection()).ToList();

            // inspect widgets for their own windows and add to viewmodel
            viewModel.Windows.AddRange(Plugin.UserInterfaceWindows());

            return viewModel;
        }

        public const string PluginDir = "~/plugins";
        public const string PluginTempDir = "~/plugins/temp";
    }
}