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
            get
            {
                return new ResourceCollection()
                  .Load("/core/js/codemirror/codemirror.js", MatchMode.Path)
                  .Load("/core/js/codemirror/runmode.js", MatchMode.Path)
                  .Load("/core/widgets/html/eyepatch-widgets-codeblock.js", MatchMode.FileName);
            }
        }

        public override ResourceCollection Css
        {
            get
            {
                return new ResourceCollection()
                  .Load("/core/js/codemirror/codemirror.css", MatchMode.Path)
                  .Load("/core/js/codemirror/default.css", MatchMode.Path);
            }
        }

        public override ResourceCollection AdminJs
        {
            get
            {
                if (adminJs == null)
                {
                    adminJs = new ResourceCollection()
                        .Load("/core/js/rangy-core.js", MatchMode.Path)
                        .Load("/core/js/rangy-selectionsaverestore.js", MatchMode.Path)
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