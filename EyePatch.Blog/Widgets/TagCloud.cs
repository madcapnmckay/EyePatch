using System.Web.Routing;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Widgets;

namespace EyePatch.Blog.Widgets
{
    public class TagCloud : PartialRequestWidget
    {
        public override RouteValueDictionary RouteValues
        {
            get
            {
                return new RouteValueDictionary(new
                                                    {
                                                        controller = "Blog",
                                                        action = "TagCloud"
                                                    });
            }
        }

        public override string Name
        {
            get { return "Tag Cloud"; }
        }

        public override object InitialContents
        {
            get { return null; }
        }

        public override string CreateFunction
        {
            get { return "ep.blog.tagcloud.create"; }
        }

        public override string CssClass
        {
            get { return "tag-cloud"; }
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
            get { return ResourceCollection.Empty; }
        }

        public override ResourceCollection AdminCss
        {
            get { return ResourceCollection.Empty; }
        }
    }
}