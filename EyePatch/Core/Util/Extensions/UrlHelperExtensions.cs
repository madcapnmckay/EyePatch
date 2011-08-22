using System.Web.Mvc;

namespace EyePatch.Core.Util.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ActionSeo(this UrlHelper urlHelper, string actionName, string controller)
        {
            return urlHelper.Action(actionName, controller).ToLowerInvariant();
        }

        public static string ActionSeo(this UrlHelper urlHelper, string actionName, string controller,
                                       object routeValues)
        {
            return urlHelper.Action(actionName, controller, routeValues).ToLowerInvariant();
        }

        public static string NormalizeUrl(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            url = url.Trim();
            url = url.StartsWith("/") ? url : string.Concat("/", url);

            return (url.Length == 1 ? url : url.TrimEnd('/')).ToLowerInvariant();
        }

        public static string FullyQualifiedApplicationPath(this UrlHelper urlHelper)
        {
            //Getting the current context of HTTP request
            var context = urlHelper.RequestContext.HttpContext;

            //Checking the current context content
            if (context == null) return null;

            //Formatting the fully qualified website url/name
            var appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        context.Request.Url.Port == 80
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }
    }
}