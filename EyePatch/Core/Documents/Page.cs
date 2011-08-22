using System;
using System.Collections.Generic;
using EyePatch.Core.Audit;
using EyePatch.Core.Mvc.Sitemap;

namespace EyePatch.Core.Documents
{
    public class Page : ISiteMapItem, IAuditCreatedDate, IAuditModifiedDate
    {
        public Page()
        {
            ContentAreas = new List<ContentArea>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public string TemplateId { get; set; }
        public IList<ContentArea> ContentAreas { get; set; }

        public string AnalyticsKey { get; set; }

        public int MenuOrder { get; set; }
        public bool IsLive { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDynamic { get; set; }

        // meta data
        public int? Language { get; set; }
        public string Author { get; set; }
        public string Charset { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Robots { get; set; }
        public string OgType { get; set; }
        public string OgEmail { get; set; }
        public string OgPhone { get; set; }
        public string OgImage { get; set; }
        public double? OgLongitude { get; set; }
        public double? OgLatitude { get; set; }
        public string OgStreetAddress { get; set; }
        public string OgLocality { get; set; }
        public string OgRegion { get; set; }
        public string OgCountry { get; set; }
        public string OgPostcode { get; set; }

        #region ISiteMapItem Members

        public string Url { get; set; }

        // sitemap
        public ChangeFrequency? ChangeFrequency { get; set; }
        public double? Priority { get; set; }

        // audit
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }

        #endregion
    }
}