using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if (result.Succeeded)
            {
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



    }
}
