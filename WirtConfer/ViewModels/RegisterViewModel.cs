using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "NameLength")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        [MinLength(2, ErrorMessage = "SurnameLength")]
        public string Surname { get; set; }
        [Range(16, 100, ErrorMessage = "AgeRange")]
        [Required(ErrorMessage = "Required")]
        public int Age { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Required")]
        public string Email { get; set; }

        [DataType(DataType.Password,ErrorMessage = "PasswordEr")]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "NotMatch")] 
        public string ConfirmPassword { get; set; }

    }
}
