using EyePatch.Core.Models;
using EyePatch.Core.Services;

namespace EyePatch.Core
{
    /// <summary>
    /// An aggregator service class that provides the overall interface to the 
    /// content management system
    /// </summary>
    public interface IContentManager
    {
        IPageService Page { get; }
        IFolderService Folder { get; }
        ITemplateService Template { get; }
        IWidgetService Widget { get; }
        IApplicationService Application { get; }
        IResourceService Resources { get; }
        IPluginService Plugin { get; }
        IMediaService Media { get; }

        string SiteName { get; }
        string SiteDescription { get; }

        AdminPanelViewModel AdminPanel(int pageId);
    }
}