using System.Web;
using System.Web.Mvc;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Blog.Models
{
    public class BlogRoutes
    {
        protected UrlHelper urlHelper;

        protected UrlHelper Url
        {
            get
            {
                if (urlHelper == null)
                {
                    var requestContext = new System.Web.Routing.RequestContext(
                                    new HttpContextWrapper(HttpContext.Current),
                                    new System.Web.Routing.RouteData());
                    urlHelper = new System.Web.Mvc.UrlHelper(requestContext);
                }
                return urlHelper;
            }
        }

        public string Info
        {
            get { return Url.ActionSeo("Info", "BlogAdmin"); }
        }

        public string Create
        {
            get { return Url.ActionSeo("Create", "BlogAdmin"); }
        }

        public string Remove
        {
            get { return Url.ActionSeo("Remove", "BlogAdmin"); }
        }

        public string Rename
        {
            get { return Url.ActionSeo("Rename", "BlogAdmin"); }
        }

        public string Update
        {
            get { return Url.ActionSeo("Update", "BlogAdmin"); }
        }

        public string Tags
        {
            get { return Url.ActionSeo("Tags", "BlogAdmin"); }
        }
        
        public string Navigate
        {
            get { return Url.ActionSeo("Navigate", "BlogAdmin"); }
        }

        public string Settings
        {
            get { return Url.ActionSeo("Settings", "BlogAdmin"); }
        }
    }
}