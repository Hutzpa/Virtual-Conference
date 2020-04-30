using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Models.States;

namespace WirtConfer.Models
{
    public class Invite
    {
        [Key]
        public int Id { get; set; }
        [Url]
        public string Url { get; set; }

        public InviteType Type { get; set; }

        public int EventId { get; set; }
        public Event_ Event { get; set; }

    }
}
