using System;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using EyePatch.Core.Services;
using EyePatch.Core.Util;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Config;
using Raven.Http;
using StructureMap.Configuration.DSL;

namespace EyePatch.Core.IoC
{
    /// <summary>
    ///   Registry for the core services used in the CMS
    /// </summary>
    public class CoreRegistry : Registry
    {
        private static IDocumentStore GetDocumentStore()
        {
            var portConfig = WebConfigurationManager.AppSettings["StudioPort"];
            int port;
            if (!Int32.TryParse(portConfig, out port))
                port = 8080;

            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(port);
            var documentStore = new EmbeddableDocumentStore
            {
                DataDirectory =
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\EyePatchDB"),
                UseEmbeddedHttpServer = true
            };

            documentStore.Conventions.IdentityPartsSeparator = "-";
            documentStore.Configuration.Port = port;
            documentStore.Initialize();

            return documentStore;
        }

        public CoreRegistry()
        {
            For<IDocumentStore>().Singleton().Use(GetDocumentStore);
            For<IDocumentSession>().HttpContextScoped().Use(x =>
            {
                var store = x.GetInstance<IDocumentStore>();
                return store.OpenSession();
            });

            For<IContentManager>().Use<ContentManager>();
            For<IPageService>().Use<PageService>();
            For<IFolderService>().Use<FolderService>();
            For<ITemplateService>().Use<TemplateService>();
            For<IWidgetService>().Use<WidgetService>();
            For<IApplicationService>().Use<ApplicationService>();
            For<IResourceService>().Use<ResourceService>();
            For<IPluginService>().Use<PluginService>();
            For<IMediaService>().Use<MediaService>();
            For<ICacheProvider>().Use<CacheProvider>();
            For<MembershipProvider>().Use(x => Membership.Provider);
            For<IMembershipService>().Use<AccountMembershipService>();
            For<IFormsAuthenticationService>().Use<FormsAuthenticationService>();
            
            For<IControllerActivator>().Use<StructureMapControllerActivator>();
            For<IControllerFactory>().Use<StructureMapControllerFactory>();
        }
    }
}