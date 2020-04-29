using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.Models
{
    public class Blacklist
    {
        public int IdEvent { get; set; }
        public Event_ Event { get; set; }  
        public string IdUser { get; set; }
        public User User { get; set; }
    }
}
