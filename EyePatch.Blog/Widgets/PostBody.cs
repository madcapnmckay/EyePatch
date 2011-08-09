using System.Reflection;
using System.Web.Routing;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Widgets;

namespace EyePatch.Blog.Widgets
{
    public class PostBody : PartialRequestWidget
    {
        private ResourceCollection adminJs;

        public override RouteValueDictionary RouteValues
        {
            get
            {
                return new RouteValueDictionary(new
                {
                    controller = "Blog",
                    action = "Post"
                });
            }
        }

        public override string Name
        {
            get { return "Post Body"; }
        }

        public override object InitialContents
        {
            get { return null; }
        }

        public override string CreateFunction
        {
            get { return "ep.blog.post.create"; }
        }

        public override string CssClass
        {
            get { return "ep-widget-blog-body"; }
        }

        public override ResourceCollection Js
        {
            get { return ResourceCollection.Empty; }
        }

        public override ResourceCollection Css
        {
            get { return ResourceCollection.Empty; }
        }

        public override ResourceCollection AdminJs
        {
            get
            {
                if (adminJs == null)
                {
                    adminJs = new ResourceCollection()
                                    .Load("/core/js/rangy-core.js", MatchMode.Path)
                                    .Load("/core/js/rangy-cssclassapplier.js", MatchMode.Path)
                                    .Load("/core/js/knockout-ui/ui-editor.js", MatchMode.Path);
                }
                return adminJs;
            }
        }

        public override ResourceCollection AdminCss
        {
            get { return ResourceCollection.Empty; }
        }
    }
}