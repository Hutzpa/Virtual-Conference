using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MinLength(2, ErrorMessage = "Name can't be less than two chars")]
        public string Name { get; set; }
        [Required]
        [MinLength(2,ErrorMessage ="Surname can't be less than two chars")]
        public string Surname { get; set; }
        [Range(16,100, ErrorMessage ="The age must be between 16 and 100")]
        public int Age { get; set; }


        public ICollection<UserInEvent> UsersInEvents { get; set; }
    }
}
