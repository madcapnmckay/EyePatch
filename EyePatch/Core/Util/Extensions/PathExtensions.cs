using System;
using System.Text.RegularExpressions;
using System.Web;

namespace EyePatch.Core.Util.Extensions
{
    public static class PathExtensions
    {
        private static readonly Regex isFullQualified = new Regex("^http(s)?://.$",
                                                                  RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string ToRelativeUrl(this string physicalPath)
        {
            if (HttpContext.Current.Request.PhysicalApplicationPath == null)
                throw new NullReferenceException("Cannot access HttpContext.Current.Request.PhysicalApplicationPath");

            return physicalPath.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "/").Replace("\\", "/");
        }

        public static bool IsFullyQualified(this string url)
        {
            return isFullQualified.IsMatch(url);
        }

        public static string ToFullyQualifiedUrl(this string url)
        {
            if (url.IsFullyQualified())
                return url;

            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + url;
        }
    }
}