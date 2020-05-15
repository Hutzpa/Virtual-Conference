using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Models;
using WirtConfer.ViewModels;

namespace WirtConfer.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        public AuthController(SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            var res = await _signInManager.PasswordSignInAsync(lvm.Email, lvm.Password,false,false);
            if(res.Succeeded)
                return RedirectToAction("Index", "Home");
            return View(lvm);
        } 


        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            if (!ModelState.IsValid)
                return View(rvm);

            var User = new User
            {
               UserName = rvm.Email,
               Name = rvm.Name,
               Surname = rvm.Surname,
               Age = rvm.Age,
               Email = rvm.Email,
            };
            var result = await _userManager.CreateAsync(User, rvm.Password);

            AuthEvent authEvent = new AuthEvent();
            authEvent.GreetingOnEmail += EmailGreeting;//Добавляем приветствие

            if (result.Succeeded)
            {
                authEvent.Greeting(rvm.Email, rvm.Name);
                await _signInManager.SignInAsync(User, false);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private void EmailGreeting(string email, string name)
        {
            try
            {
                MailAddress from = new MailAddress("illia.bezuhlyi@nure.ua", "Wirtual conference");
                MailAddress to = new MailAddress(email);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Welcome";
                m.Body = name + " welcome in Wirtual conferenct service";
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("illia.bezuhlyi@nure.ua", "TLnK5nd3");
                smtp.EnableSsl = true;
                smtp.Send(m);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
