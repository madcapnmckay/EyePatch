using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //get request and response
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;

            //get encoding
            var encoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrWhiteSpace(encoding))
                return;

            encoding = encoding.ToUpperInvariant();


            if (encoding.Contains("DEFLATE") || encoding == "*")
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}