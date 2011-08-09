using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EyePatch.Core.Entity;

namespace EyePatch.Core.Widgets
{
    public class WidgetContext
    {
        public HttpContextBase HttpContext { get; protected set; }
        public RouteData RouteData { get; protected set; }
        public WidgetInstance Instance { get; protected set; }
        public TextWriter Writer { get; protected set; }

        public WidgetContext(ControllerContext controllerContext, WidgetInstance pageWidget, TextWriter writer)
        {
            HttpContext = controllerContext.HttpContext;
            RouteData = controllerContext.RouteData;
            Instance = pageWidget;
            Writer = writer;
        }
    }
}