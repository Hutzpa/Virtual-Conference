using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer
{
    public class AuthEvent
    {
        public delegate void SendMes(string email, string name);
        public event SendMes GreetingOnEmail;
        public bool Greeting(string email,string name)
        {
            if (GreetingOnEmail == null)
                return false;
            GreetingOnEmail.Invoke(email, name);
            return true;
        }     
    }
}
