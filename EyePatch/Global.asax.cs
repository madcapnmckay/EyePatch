using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using EyePatch.Core;
using EyePatch.Core.IoC;
using EyePatch.Core.Mvc;
using EyePatch.Core.Mvc.ActionFilters;
using EyePatch.Core.Mvc.Routing;
using EyePatch.Core.Util;
using EyePatch.Core.Widgets;
using Raven.Client;
using StructureMap;

namespace EyePatch
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HandleJsonErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*allcss}", new {allcss = @".*\.css(/.*)?"});
            routes.IgnoreRoute("{*alljs}", new {allcss = @".*\.js(/.*)?"});
            routes.IgnoreRoute("{*allaspx}", new {allaspx = @".*\.aspx(/.*)?"});
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Sitemap", "sitemap.xml", new {controller = "Content", action = "SiteMap"});
            routes.MapRoute("Robots", "robots.txt", new {controller = "Content", action = "Robots"});

            // include all plugin routes
            foreach (var registry in ObjectFactory.Model.GetAllPossible<IRouteRegistry>())
            {
                registry.RegisterRoutes(routes);
            }

            routes.MapRoute("Admin", "admin", new {controller = "Admin", action = "SignIn"});
            routes.MapRoute("AdminPanel", "admin-panel/{pageId}", new {controller = "Admin", action = "Panel"});
            routes.MapRoute("SignOut", "signout", new {controller = "Admin", action = "SignOut"});

            routes.MapEyePatchRoute(
                "EyePatch", // Route name
                "{controller}/{action}", // URL with parameters
                new {controller = "Admin", action = "Install"}, // Parameter defaults
                null, null
                );

            routes.MapRoute("Default", "{controller}/{action}/{id}", new {id = UrlParameter.Optional});
        }

        protected void Application_Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += PluginAssemblyResolve;
            HostingEnvironment.RegisterVirtualPathProvider(new AssemblyResourceProvider());
            
            EyePatchApplication.Startup();

            // MVC components
            DependencyResolver.SetResolver(new StructureMapDependencyResolver());
            ViewEngines.Engines.Add(new EyePatchViewEngine());
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_End()
        {
            var documentStore = ObjectFactory.GetInstance<IDocumentStore>();
            documentStore.Dispose();
        }

        protected void Application_EndRequest()
        {
            ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();
        }

        private Assembly PluginAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Check we don't already have the assembly loaded
            foreach (var assembly in currentAssemblies)
            {
                if (assembly.FullName == args.Name || assembly.GetName().Name == args.Name)
                {
                    return assembly;
                }
            }

            // Load from directory
            return LoadAssemblyFromPath(args.Name, Server.MapPath(ContentManager.PluginDir));
        }

        private static Assembly LoadAssemblyFromPath(string assemblyName, string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                Assembly assembly;

                if (TryLoadAssemblyFromFile(file, assemblyName, out assembly))
                {
                    return assembly;
                }
            }

            return null;
        }

        private static bool TryLoadAssemblyFromFile(string file, string assemblyName, out Assembly assembly)
        {
            // Convert the filename into an absolute file name for
            // use with LoadFile.
            file = new FileInfo(file).FullName;

            if (AssemblyName.GetAssemblyName(file).Name == assemblyName)
            {
                assembly = Assembly.LoadFile(file);
                return true;
            }

            assembly = null;
            return false;
        }

        
    }
}