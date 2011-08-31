using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Widgets
{
    public abstract class PartialRequestWidget : IWidget
    {
        public abstract RouteValueDictionary RouteValues { get; }

        #region IWidget Members

        public abstract string Name { get; }
        public abstract object InitialContents { get; }
        public abstract string CreateFunction { get; }
        public abstract string CssClass { get; }
        public abstract ResourceCollection Js { get; }
        public abstract ResourceCollection Css { get; }
        public abstract ResourceCollection AdminJs { get; }
        public abstract ResourceCollection AdminCss { get; }

        public void Render(WidgetContext context)
        {
            var rd = new RouteData(context.RouteData.Route, context.RouteData.RouteHandler);

            // add route values for the widget;
            foreach (var p1 in RouteValues)
                rd.Values.Add(p1.Key, p1.Value);

            // add all existing route data values that doesn't clash
            foreach (var p2 in context.RouteData.Values.Where(r => !rd.Values.ContainsKey(r.Key)))
                rd.Values.Add(p2.Key, p2.Value);

            // add page to the route data
            rd.Values.Add("sourceUrl", context.SourceUrl);

            // add widget to the route data
            //rd.Values.Add("instance", Widget);

            //setup httpcontext
            // allows us to mock it
            var req = new HttpRequest(context.HttpContext.Request.Path,
                                      context.HttpContext.Request.Url.AbsoluteUri,
                                      context.HttpContext.Request.Url.Query.TrimStart('?'));
            var res = new HttpResponse(context.Writer);
            var ctx = new HttpContext(req, res);
            ctx.User = context.HttpContext.User;

            IHttpHandler handler = new MvcHandler(new RequestContext(new HttpContextWrapper(ctx), rd));

            handler.ProcessRequest(ctx);
        }

        #endregion
    }
}