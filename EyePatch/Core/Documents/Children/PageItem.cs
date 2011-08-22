namespace EyePatch.Core.Documents.Children
{
    public class PageItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int MenuOrder { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsLive { get; set; }
        public bool IsHomePage { get; set; }
    }
}