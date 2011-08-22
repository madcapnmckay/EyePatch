using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using EyePatch.Core.Util.Extensions;

namespace EyePatch.Core.Mvc.Sitemap
{
    public static class XmlSiteMap
    {
        private const string Url = "url";
        private const string UrlSet = "urlset";
        private const string UrlSetSchemaLocation = "schemaLocation";
        private const string UrlXsi = "xsi";

        private const string UrlSetSchemaLocationUrl =
            "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";

        private static readonly XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

        public static string Create(IEnumerable<ISiteMapItem> items)
        {
            var xDoc =
                new XDocument(new XDeclaration("1.0", HttpContext.Current.Response.ContentEncoding.WebName, "yes"),
                              new XElement(xmlns + UrlSet,
                                           new XAttribute(XNamespace.Xmlns + UrlXsi, xsi),
                                           new XAttribute(xsi + UrlSetSchemaLocation, UrlSetSchemaLocationUrl),
                                           items.Select(CreateSitemapNode)
                                  ));

            return (xDoc.Declaration + xDoc.ToString());
        }

        private static XElement CreateSitemapNode(ISiteMapItem item)
        {
            var itemElement = new XElement(xmlns + Url);

            itemElement.Add(new XElement(xmlns + "loc",
                                         item.Url.ToLowerInvariant().ToFullyQualifiedUrl()));

            if (item.LastModified.HasValue)
                itemElement.Add(new XElement(xmlns + "lastmod",
                                             CreateXmlDate(item.LastModified)));

            if (item.ChangeFrequency.HasValue)
                itemElement.Add(new XElement(xmlns + "changefreq",
                                             item.ChangeFrequency.Value.ToString().ToLowerInvariant()));

            if (item.Priority.HasValue)
                itemElement.Add(new XElement(xmlns + "priority",
                                             item.Priority.Value.ToString(CultureInfo.InvariantCulture)));

            return itemElement;
        }

        private static string CreateXmlDate(DateTime? date)
        {
            return !date.HasValue
                       ? string.Empty
                       : string.Format("{0}-{1}-{2}T{3}:{4}:{5}+00:00",
                                       new object[]
                                           {
                                               date.Value.Year, date.Value.Month.ToString("00"),
                                               date.Value.Day.ToString("00"), date.Value.Hour.ToString("00"),
                                               date.Value.Minute.ToString("00"), date.Value.Second.ToString("00")
                                           });
        }
    }
}