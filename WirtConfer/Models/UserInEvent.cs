using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Models.States;

namespace WirtConfer.Models
{
    public class UserInEvent
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event_ Event { get; set; }

        public bool IsBanned { get; set; }

        [Required]
        public Roles Role { get; set; }
    }
}
