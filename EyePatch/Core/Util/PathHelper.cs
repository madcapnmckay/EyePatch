using System.Web;

namespace EyePatch.Core.Util
{
    public static class PathHelper
    {
        public static string PhysicalToUrl(string physicalPath)
        {
            var rootpath = HttpContext.Current.Server.MapPath("~/");
            var url = physicalPath.Replace(rootpath, "");
            url = url.Replace("\\", "/");
            return "/" + url;
        }

        public static string CombineUrl(string first, string second)
        {
            if (first.Length == 0)
                return second;

            if (second.Length == 0)
                return first;

            first = first.TrimEnd('/');
            second = second.TrimStart('/');

            return string.Format("{0}/{1}", first, second);
        }
    }
}