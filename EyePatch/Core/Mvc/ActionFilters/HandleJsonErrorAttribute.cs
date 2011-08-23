using System;
using System.Web.Configuration;
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
                var debugMode = EyePatchApplication.ReleaseMode != ReleaseMode.Production ||
                                (WebConfigurationManager.AppSettings["ShowAjaxErrors"] == "true");

                if (!debugMode)
                {

                    data =
                        new
                            {
                                status = false,
                                message =
                                    filterContext.Exception is ApplicationException
                                        ? filterContext.Exception.Message
                                        : "An unknown error has occurred"
                            };
                }
                else
                {
                    data =
                                new
                                    {
                                        status = false,
                                        message = filterContext.Exception.Message,
                                        stackTrace = filterContext.Exception.StackTrace
                                    };
                }

                filterContext.Result = new JsonNetResult(data);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}