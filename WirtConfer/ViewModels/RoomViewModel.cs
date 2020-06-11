using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.ViewModels
{
    public class RoomViewModel
    {
        public int IdEvent { get; set; }
        public int IdRoom { get; set; }
        [Required(ErrorMessage = "NameRequired")]
        [MinLength(2, ErrorMessage = "NameLength")]
        public string RoomName { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
    }
}
