using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EyePatch.Core.Util
{
    public static class Url
    {
        public static string Combine(string baseUrl, string relativeUrl)
        {
            var resolvedBase = baseUrl;
            if (baseUrl.StartsWith("~/"))
                resolvedBase = VirtualPathUtility.ToAbsolute(baseUrl);

            if (!resolvedBase.EndsWith("/"))
                resolvedBase += "/";

            var test =  VirtualPathUtility.Combine(resolvedBase, relativeUrl);
            return test;
        }
    }
}