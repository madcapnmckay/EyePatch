using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Util.Extensions
{
    public static class ResourceExtensions
    {
        /// <summary>
        /// Takes a file and returns the min file path
        /// </summary>
        /// <returns></returns>
        public static string ToMinPath(this string path)
        {
            var extension = Path.GetExtension(path);
            if (extension == null)
                return path + ".min";

            return path.Replace(extension, string.Format(".min{0}", extension));
        }

        public static string ToTags(this IEnumerable<ResourcePath> paths)
        {
            var builder = new StringBuilder();
            foreach (var resourcePath in paths)
            {
                builder.AppendLine(resourcePath.ContentType == "text/javascript"
                                       ? BuildJs(resourcePath)
                                       : BuildCss(resourcePath));
                
            }
            return builder.ToString();
        }

        private static string BuildCss(ResourcePath resourcePath)
        {
            var scriptTag = new TagBuilder("script");
            scriptTag.Attributes["type"] = resourcePath.ContentType;
            scriptTag.Attributes["href"] = resourcePath.Url;
            scriptTag.Attributes["rel"] = "stylesheet";
            return scriptTag.ToString(TagRenderMode.SelfClosing);
        }

        private static string BuildJs(ResourcePath resourcePath)
        {
            var scriptTag = new TagBuilder("script");
            scriptTag.Attributes["type"] = resourcePath.ContentType;
            scriptTag.Attributes["src"] = resourcePath.Url;
            return scriptTag.ToString(TagRenderMode.Normal);
        }
    }
}