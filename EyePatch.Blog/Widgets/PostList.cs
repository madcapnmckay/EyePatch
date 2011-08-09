using System;
using System.Web.Routing;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Widgets;

namespace EyePatch.Blog.Widgets
{
    public class PostList : PartialRequestWidget
    {
        public override RouteValueDictionary RouteValues
        {
            get
            {
                return new RouteValueDictionary(new
                {
                    controller = "Blog",
                    action = "List"
                });
            }
        }

        public override string Name
        {
            get { return "Post List"; }
        }

        public override object InitialContents
        {
            get { return null; }
        }

        public override string CreateFunction
        {
            get { return "ep.blog.list.create"; }
        }

        public override string CssClass
        {
            get { return "blog-list"; }
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