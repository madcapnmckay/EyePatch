namespace EyePatch.Core.Documents.Children
{
    public class Widget
    {
        public Widget(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Contents { get; set; }
    }
}