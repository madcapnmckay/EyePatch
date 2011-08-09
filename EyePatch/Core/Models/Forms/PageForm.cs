using System.ComponentModel.DataAnnotations;

namespace EyePatch.Core.Models.Forms
{
    public class PageForm
    {
        public int Id { get; set; }

        public bool IsLive { get; set; }

        public bool IsInMenu { get; set; }

        public int MenuOrder { get; set; }

        [Required]
        public int TemplateID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        public bool UrlEditable { get; set; }

        public PageForm()
        {
            UrlEditable = true;
        }
    }
}