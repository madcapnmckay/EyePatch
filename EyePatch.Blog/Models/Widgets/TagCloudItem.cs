namespace EyePatch.Blog.Models.Widgets
{
    public class TagCloudItem
    {
        public string Tag { get; set; }

        public string TagTitle
        {
            get { return string.Format("Click to view posts tagged {0}", Tag); }
        }

        public int Count { get; set; }
    }
}