using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EyePatch.Core.Util
{
    public static class EmbeddedResourceTools
    {
        public static string FileContents(string fileName)
        {
            return FileContents(fileName, Assembly.GetExecutingAssembly());
        }

        public static string FileContents(string fileName, Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            var fullFileName =
                assembly.GetManifestResourceNames().Where(n => string.Compare(n, fileName, true) == 0).First();

            var fileStream = new StreamReader(assembly.GetManifestResourceStream(fullFileName));

            return fileStream.ReadToEnd();
        }
    }
}