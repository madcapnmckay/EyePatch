using System.Web.Mvc;
using System.Web.Security;
using EyePatch.Core.Services;
using EyePatch.Core.Entity;
using EyePatch.Core.Util;
using StructureMap.Configuration.DSL;

namespace EyePatch.Core.IoC
{
    /// <summary>
    /// Registry for the core services used in the CMS
    /// </summary>
    public class CoreRegistry: Registry
    {
        public CoreRegistry()
        {
            For<IContentManager>().Use<ContentManager>();
            For<IPageService>().Use<PageService>();
            For<IFolderService>().Use<FolderService>();
            For<ITemplateService>().Use<TemplateService>();
            For<IWidgetService>().Use<WidgetService>();
            For<IApplicationService>().Use<ApplicationService>();
            For<IMembershipService>().Use<AccountMembershipService>();
            For<IFormsAuthenticationService>().Use<FormsAuthenticationService>();
            For<IResourceService>().Use<ResourceService>();
            For<IPluginService>().Use<PluginService>();
            For<IMediaService>().Use<MediaService>();
            For<ICacheProvider>().Use<CacheProvider>();
            For<MembershipProvider>().Use(Membership.Provider);
            For<EyePatchDataContext>().HybridHttpOrThreadLocalScoped().Use(() => new EyePatchDataContext());

            For<IControllerActivator>().Use<StructureMapControllerActivator>();
            For<IControllerFactory>().Use<StructureMapControllerFactory>();
        }
    }
}