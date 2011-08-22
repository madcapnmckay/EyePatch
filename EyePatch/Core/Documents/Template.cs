namespace EyePatch.Core.Documents
{
    public class Template
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ViewPath { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsDefault { get; set; }
        public string AnalyticsKey { get; set; }

        // meta info defaults
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
    }
}