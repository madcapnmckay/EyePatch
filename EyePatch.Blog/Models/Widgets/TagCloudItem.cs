using EyePatch.Blog.Documents;

namespace EyePatch.Blog.Models.Widgets
{
    public class TagCloudItem
    {
        public Tag Tag { get; set; }

        public string TagTitle
        {
            get { return string.Format("{0} post{1} tagged {2}", Count, Count == 1 ? "" : "s", Tag.Value); }
        }
      
        public int Count { get; set; }
    }
}