using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using EyePatch.Core.Plugins;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("EyePatch CMS")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("EyePatch CMS")]
[assembly: AssemblyCopyright("Copyright © Ian Mckay 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: PreApplicationStartMethod(
    typeof (PreApplicationInit), "Initialize")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("83765420-2bc7-45cd-a088-995579ad6f25")]
[assembly: AssemblyVersion("0.4")]
[assembly: AssemblyFileVersion("0.4")]