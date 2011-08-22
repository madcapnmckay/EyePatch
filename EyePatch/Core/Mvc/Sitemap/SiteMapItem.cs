using System;

namespace EyePatch.Core.Mvc.Sitemap
{
    public class SiteMapItem : ISiteMapItem
    {
        public SiteMapItem(string url)
        {
            Url = url;
        }

        #region ISiteMapItem Members

        public string Url { get; set; }

        public DateTime? LastModified { get; set; }

        public ChangeFrequency? ChangeFrequency { get; set; }

        public double? Priority { get; set; }

        #endregion
    }
}