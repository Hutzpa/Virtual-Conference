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
        [DataType(DataType.EmailAddress,ErrorMessage = "Correct your email")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "Email can't be empty")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
