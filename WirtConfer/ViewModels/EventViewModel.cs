using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.ViewModels
{
    public class EventViewModel
    {
        public int IdEvent { get; set; }
        [Required(ErrorMessage = "NameRequired")]
        [MinLength(2, ErrorMessage = "NameLength")]
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public IFormFile Image { get; set; }

    }
}
