using System;
using System.Collections.Generic;
using EyePatch.Core.Mvc.Resources;
using EyePatch.Core.Mvc.Sitemap;
using EyePatch.Core.Widgets;
using NKnockoutUI.Window;

namespace EyePatch.Core.Plugins
{
    public class Standard : IEyePatchPlugin
    {
        #region IEyePatchPlugin Members

        public string Name
        {
            get { return "Standard"; }
        }

        public IList<Type> Widgets
        {
            get { return new List<Type> {typeof (BasicHtmlWidget), typeof (SimpleMenulWidget)}; }
        }

        public IList<Window> Windows
        {
            get { return new List<Window>(); }
        }

        public ResourceCollection Js
        {
            get { return ResourceCollection.Empty; }
        }

        public ResourceCollection Css
        {
            get { return ResourceCollection.Empty; }
        }

        public string Author
        {
            get { return "Ian Mckay"; }
        }

        public void Startup()
        {
            // nothing to do
            return;
        }

        public IEnumerable<ISiteMapItem> SiteMapItems()
        {
            return null;
        }

        #endregion
    }
}