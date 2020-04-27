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
    public class EventController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        public EventController(SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Room()
        {
            var User = await _userManager.GetUserAsync(this.User);
            return View(new UserViewModel { Name = User.Name, Surname = User.Surname });
        }
    }
}
