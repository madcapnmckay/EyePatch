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
                        .Load("/core/js/codemirror/codemirror.js", MatchMode.Path)
                        .Load("/core/js/codemirror/clike/clike.js", MatchMode.Path)
                        .Load("/core/js/codemirror/css/css.js", MatchMode.Path)
                        .Load("/core/js/codemirror/diff/diff.js", MatchMode.Path)
                        .Load("/core/js/codemirror/haskell/haskell.js", MatchMode.Path)
                        .Load("/core/js/codemirror/htmlmixed/htmlmixed.js", MatchMode.Path)
                        .Load("/core/js/codemirror/javascript/javascript.js", MatchMode.Path)
                        .Load("/core/js/codemirror/lua/lua.js", MatchMode.Path)
                        .Load("/core/js/codemirror/php/php.js", MatchMode.Path)
                        .Load("/core/js/codemirror/plsql/plsql.js", MatchMode.Path)
                        .Load("/core/js/codemirror/python/python.js", MatchMode.Path)
                        .Load("/core/js/codemirror/rst/rst.js", MatchMode.Path)
                        .Load("/core/js/codemirror/scheme/scheme.js", MatchMode.Path)
                        .Load("/core/js/codemirror/python/python.js", MatchMode.Path)
                        .Load("/core/js/codemirror/smalltalk/smalltalk.js", MatchMode.Path)
                        .Load("/core/js/codemirror/sparql/sparql.js", MatchMode.Path)
                        .Load("/core/js/codemirror/stex/stex.js", MatchMode.Path)
                        .Load("/core/js/codemirror/xml/xml.js", MatchMode.Path)
                        .Load("/core/js/codemirror/yaml/yaml.js", MatchMode.Path)
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