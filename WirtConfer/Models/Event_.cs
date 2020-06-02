using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Models.States;

namespace WirtConfer.Models
{
    public class Event_
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Name can't be less than two chars")]
        public string Name { get; set; }
        public string OwnerId { get; set; }

        public string Image { get; set; } = "";

        public ICollection<Room> Rooms { get; set; }
        public ICollection<UserInEvent> UsersInEvents { get; set; }
        public ICollection<Invite> Invites { get; set; }
    }
}
