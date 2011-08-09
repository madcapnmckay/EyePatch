using System.ComponentModel.DataAnnotations;

namespace EyePatch.Blog.Models.Forms
{
    public class PostForm
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        public string Tags { get; set; }

        public bool Published { get; set; }
    }
}