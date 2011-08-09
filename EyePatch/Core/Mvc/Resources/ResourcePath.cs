using Newtonsoft.Json;

namespace EyePatch.Core.Mvc.Resources
{
    public class ResourcePath
    {
        public string Url { get; set; }

        public string ContentType { get; set; }

        [JsonIgnore]
        public string Contents { get; set; }

        [JsonIgnore]
        public string FileName { get; set; }
    }
}