using System.Web.Routing;
using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Widgets
{
    public class SimpleMenulWidget : PartialRequestWidget
    {
        private ResourceCollection adminJs;

        public override RouteValueDictionary RouteValues
        {
            get
            {
                return new RouteValueDictionary(new
                                                    {
                                                        controller = "Menu",
                                                        action = "TopLevel"
                                                    });
            }
        }

        public override string Name
        {
            get { return "Simple Menu"; }
        }

        public override object InitialContents
        {
            get { return "No menu items"; }
        }

        public override string CreateFunction
        {
            get { return "ep.widgetTypes.menu.create"; }
        }

        public override string CssClass
        {
            get { return "ep-widget-menu"; }
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
                        .Load("/core/widgets/simplemenu/eyepatch-widgets-menu-admin.js", MatchMode.Path);
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