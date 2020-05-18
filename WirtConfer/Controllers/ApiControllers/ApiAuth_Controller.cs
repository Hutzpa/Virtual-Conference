using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WirtConfer.Models;

namespace WirtConfer.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAuth_Controller : ControllerBase
    {
        private SignInManager<User> _signInManager;

        public ApiAuth_Controller(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        // GET: api/ApiAuth_/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<bool> GetAsync(string login, string password)
        {
            var res = await _signInManager.PasswordSignInAsync(login, password, false, false);
            if (res.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
