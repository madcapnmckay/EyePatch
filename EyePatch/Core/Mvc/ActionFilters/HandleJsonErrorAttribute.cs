using System;
using System.Web.Mvc;
using EyePatch.Core.Mvc.ActionResults;
using EyePatch.Core.Util;

namespace EyePatch.Core.Mvc.ActionFilters
{
    public class HandleJsonErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentException("filterContext");

            if (!filterContext.ExceptionHandled && filterContext.HttpContext.Request.IsAjaxRequest())
            {
                object data;
                // we only include the stacktrace when in debug mode
                switch (EyePatchApplication.ReleaseMode)
                {
                    case ReleaseMode.Production :
                        data =
                            new
                            {
                                status = false,
                                message =
                                    filterContext.Exception is ApplicationException
                                        ? filterContext.Exception.Message
                                        : "An unknown error has occurred"
                            };
                        break;
                    default:
                        data = new { status = false, message = filterContext.Exception.Message, stackTrace = filterContext.Exception.StackTrace };
                        break;
                }

                filterContext.Result = new JsonNetResult(data);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}
