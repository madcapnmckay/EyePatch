using System.Collections.Generic;
using EyePatch.Core.Mvc.Resources;
using Newtonsoft.Json;

namespace EyePatch.Core.Models
{
    public class WidgetInstance
    {
        public int Id { get; set; }
        public string CssClass { get; set; }
        public object Contents { get; set; }
        [JsonProperty(PropertyName = "~initializeFunction")]
        public string InitializeFunction { get; set; }

        public IList<ResourcePath> Js { get; set; }
        public IList<ResourcePath> Css { get; set; }
    }
}