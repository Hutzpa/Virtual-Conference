using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.Models
{
    public class Invite
    {
        [Key]
        public int Id { get; set; }
        [Url]
        public string Url { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }

    }
}
