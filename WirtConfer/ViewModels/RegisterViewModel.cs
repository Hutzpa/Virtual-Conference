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
        [Required]
        [MinLength(2, ErrorMessage = "Name can't be less than two chars")]
        public string Name { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Surname can't be less than two chars")]
        public string Surname { get; set; }
        [Range(16, 100, ErrorMessage = "The age must be between 16 and 100")]
        [Required]
        public int Age { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")] 
        public string ConfirmPassword { get; set; }

    }
}
