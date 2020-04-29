using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Data;
using WirtConfer.Models;
using WirtConfer.ViewModels;

namespace WirtConfer.Controllers
{
    public class EventController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;

        public EventController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> Room()
        {
            var User = await _userManager.GetUserAsync(this.User);
            return View(new RoomViewModel { Name = User.Name, Surname = User.Surname, IdEvent = "12", IdRoom = "34" });
        }


        [HttpGet]
        public IActionResult CreateEv() => View(new EventViewModel());

        [HttpPost]
        public async Task<IActionResult> CreateEv(EventViewModel evm)
        {
            var curUsr = await _userManager.GetUserAsync(this.User);
            var Event = new Event_
            {
                Name = evm.Name,
                OwnerId = curUsr.Id
            };
            await _dbContext.Events.AddAsync(Event);
            var saveRes = await _dbContext.SaveChangesAsync();

            if (saveRes != 0)
                return RedirectToAction("EventList", "Event");

            return View(evm);

        }

        [HttpGet]
        public IActionResult EventList(string UserId = null)
        {
            var userId = UserId ?? _userManager.GetUserId(HttpContext.User);
            return View(_dbContext.Events.Where(o => o.OwnerId == userId).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvAsync(int id)
        {
            _dbContext.Events.Remove(_dbContext.Events.FirstOrDefault(o => o.Id == id));
            var saveRes = await _dbContext.SaveChangesAsync();
            if (saveRes != 0)
                return RedirectToAction("EventList", "Event");
            else
                return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> EventAsync(int id)
        {
            var ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == id);
            return View(new EventViewModel { IdEvent = ev.Id, Name = ev.Name });
        }



        //[HttpPost]
        //public IActionResult DeleteRoom(EventViewModel evm)
        //{
        //    _dbContext.Events.Remove(_dbContext.Events.FirstOrDefault(o => o.Id == evm.IdEvent));
        //    return RedirectToAction("Event", "Event");
        //}





    }
}
