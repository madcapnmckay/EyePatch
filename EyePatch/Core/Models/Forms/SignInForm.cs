using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EyePatch.Core.Models.Forms
{
    public class SignInForm
    {
        [Required, DisplayName("User Name")]
        public string UserName { get; set; }

        [Required, DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}