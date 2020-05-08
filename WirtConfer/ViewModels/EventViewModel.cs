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
        [Required]
        [MinLength(2, ErrorMessage = "Name can't be less than two chars")]
        public string Name { get; set; }
        public string OwnerId { get; set; }

    }
}
