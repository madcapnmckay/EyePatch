using System.ComponentModel.DataAnnotations;

namespace EyePatch.Core.Models.Forms
{
    public class PageForm
    {
        public PageForm()
        {
            UrlEditable = true;
        }

        public string Id { get; set; }

        public bool IsLive { get; set; }

        public bool IsInMenu { get; set; }

        public int MenuOrder { get; set; }

        [Required]
        public string TemplateID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        public bool UrlEditable { get; set; }
    }
}