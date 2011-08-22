using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Widgets
{
    public class BasicHtmlWidget : IWidget
    {
        protected const string defaultContents = "Click here to edit this text";

        private ResourceCollection adminCss;
        private ResourceCollection adminJs;

        #region IWidget Members

        public string Name
        {
            get { return "Text/Html"; }
        }

        public object InitialContents
        {
            get { return defaultContents; }
        }

        public string CreateFunction
        {
            get { return "ep.widgetTypes.html.create"; }
        }

        public string CssClass
        {
            get { return "ep-widget-html"; }
        }

        public ResourceCollection Js
        {
            get { 
                return new ResourceCollection()
                    .Load("/core/js/codemirror/codemirror.js", MatchMode.Path)
                    .Load("/core/js/codemirror/runmode.js", MatchMode.Path)
                    .Load("/core/widgets/html/eyepatch-widgets-codeblock.js", MatchMode.FileName); 
            }
        }

        public ResourceCollection Css
        {
            get
            {
                return new ResourceCollection()
                    .Load("/core/js/codemirror/codemirror.css", MatchMode.Path)
                    .Load("/core/js/codemirror/default.css", MatchMode.Path);
            }
        }

        public ResourceCollection AdminJs
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
                        .Load("/core/js/knockout-ui/ui-editor.js", MatchMode.Path)
                        .Load("/core/widgets/html/eyepatch-widgets-html-admin.js", MatchMode.FileName);
                }
                return adminJs;
            }
        }

        public ResourceCollection AdminCss
        {
            get
            {
                if (adminCss == null)
                {
                    adminCss = new ResourceCollection()
                        .Load("/core/js/codemirror/codemirror.css", MatchMode.FileName)
                        .Load("/core/js/codemirror/default.css", MatchMode.FileName);
                }
                return adminCss;
            }
        }

        public void Render(WidgetContext context)
        {
            context.Writer.Write(string.IsNullOrWhiteSpace(context.Instance.Contents)
                                     ? defaultContents
                                     : context.Instance.Contents);
        }

        #endregion

        public void Startup()
        {
            // nothin to do
        }

        public void Register()
        {
            // nothing to do
        }
    }
}