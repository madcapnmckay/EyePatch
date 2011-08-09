using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using EyePatch.Core.Util.Files;

namespace EyePatch.Core.Plugins
{
    /// <summary>
    /// Loads plugin types so the runtime can load them
    /// http://shazwazza.com/post/Developing-a-plugin-framework-in-ASPNET-with-medium-trust.aspx
    /// </summary>
    public class PreApplicationInit
    {
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        static PreApplicationInit()
        {
            pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath(ContentManager.PluginDir));
            shadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath(ContentManager.PluginTempDir));
        }

        /// <summary>
        /// The source plugin folder from which to shadow copy from
        /// </summary>
        /// <remarks>
        /// This folder can contain sub folderst to organize plugin types
        /// </remarks>
        private static readonly DirectoryInfo pluginFolder;

        /// <summary>
        /// The folder to shadow copy the plugin DLLs to use for running the app
        /// </summary>
        private static readonly DirectoryInfo shadowCopyFolder;

        public static void Initialize()
        {
            using (new WriteLockDisposable(Locker))
            {

                Directory.CreateDirectory(shadowCopyFolder.FullName);

                //clear out plugins, need to be renamed
                foreach (var f in shadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    var filename = Path.Combine(f.DirectoryName, f.Name + ".old");
                    try
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                    } catch(UnauthorizedAccessException ex)
                    {

                    }

                    File.Move(f.FullName, filename);
                }

                //shadow copy files
                foreach (var plug in pluginFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    var di = Directory.CreateDirectory(Path.Combine(shadowCopyFolder.FullName, plug.Directory.Name));
                    // NOTE: You cannot rename the plugin DLL to a different name, it will fail because the assembly name is part if it's manifest
                    // (a reference to how assemblies are loaded: http://msdn.microsoft.com/en-us/library/yx7xezcf )
                    File.Copy(plug.FullName, Path.Combine(di.FullName, plug.Name), true);
                }

                // Now, we need to tell the BuildManager that our plugin DLLs exists and to reference them.
                // There are different Assembly Load Contexts that we need to take into account which 
                // are defined in this article here:
                // http://blogs.msdn.com/b/suzcook/archive/2003/05/29/57143.aspx

                // * This will put the plugin assemblies in the 'Load' context
                // This works but requires a 'probing' folder be defined in the web.config
                foreach (var dll in shadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    var filename = dll.FullName;
                    var assemblyName = AssemblyName.GetAssemblyName(filename);
                    var assembly = Assembly.Load(assemblyName);

                    BuildManager.AddReferencedAssembly(assembly);
                }
            }
        }
    }
}