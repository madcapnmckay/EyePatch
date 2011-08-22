using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EyePatch.Core.Documents.Children;

namespace EyePatch.Core.Widgets
{
    public class WidgetContext
    {
        public WidgetContext(ControllerContext controllerContext, Widget pageWidget, TextWriter writer, string sourceUrl)
        {
            HttpContext = controllerContext.HttpContext;
            RouteData = controllerContext.RouteData;
            Instance = pageWidget;
            Writer = writer;
            SourceUrl = sourceUrl;
        }

        public HttpContextBase HttpContext { get; protected set; }
        public RouteData RouteData { get; protected set; }
        public Widget Instance { get; protected set; }
        public TextWriter Writer { get; protected set; }
        public string SourceUrl { get; protected set; }
    }
}