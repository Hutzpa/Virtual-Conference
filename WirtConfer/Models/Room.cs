using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set;  }

        [Required]
        [MinLength(2,ErrorMessage = "Room name can't be less than two characters")]
        public string Name { get; set; }

    }
}
