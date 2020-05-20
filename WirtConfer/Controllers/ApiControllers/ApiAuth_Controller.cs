using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WirtConfer.Data;
using WirtConfer.Models;

namespace WirtConfer.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAuth_Controller : ControllerBase
    {
        private SignInManager<User> _signInManager;
        private ApplicationDbContext _context;

        public ApiAuth_Controller(SignInManager<User> signInManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/ApiAuth_/5
        [HttpGet]
        public async Task<string> GetAsync(string login, string password)
        {
            
            var res = await _signInManager.PasswordSignInAsync(login, password, false, false);
            if (res.Succeeded)
            {
                User user = await _context.Users.FirstOrDefaultAsync(o => o.Email == login);
                return user.Id;
            }
            return "";
        }
    }
}
