using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class CompressAttribute : ActionFilterAttribute
    {
        protected static Regex adminPanelRegex = new Regex("</head>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        }
    }
}