using System;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Core.Mvc.Sitemap
{
    public class SiteMapItem : ISiteMapItem
    {
        public SiteMapItem(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public DateTime? LastModified { get; set; }

        public ChangeFrequency? ChangeFrequency { get; set; }

        public double? Priority { get; set; }
    }
}