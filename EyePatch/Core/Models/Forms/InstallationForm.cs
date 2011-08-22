using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EyePatch.Core.Models.Forms
{
    public class InstallationForm
    {
        [Required(ErrorMessage = "A sitename must be specified")]
        [Display(Name = "Site Name", Description = "The name of the site you wish to create")]
        public string SiteName { get; set; }

        [Required(ErrorMessage = "A username must be supplied")]
        [Display(Name = "Administrator Username", Description = "The username you will use to login to EyePatch")]
        [StringLength(25, ErrorMessage = "The username must be under 25 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A password must be supplied")]
        [Display(Name = "Administrator Password", Description = "The password you will use to login to EyePatch")]
        [StringLength(25, MinimumLength = 6,
            ErrorMessage = "The password must be between 6 and 25 characters in length.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "A password must be supplied")]
        [Display(Name = "Password Confirmation")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords must match")]
        public string ComparePassword { get; set; }

        [Required(ErrorMessage = "An email address must be supplied")]
        [Display(Name = "Administrator Email", Description = "An email address required to send password resets.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
    }
}