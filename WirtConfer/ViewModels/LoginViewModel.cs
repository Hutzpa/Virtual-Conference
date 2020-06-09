using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.ViewModels
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress,ErrorMessage = "Email")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "EmailRequired")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }
    }
}
