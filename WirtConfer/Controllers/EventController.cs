using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Data;
using WirtConfer.Data.FileManager;
using WirtConfer.Data.Repositories;
using WirtConfer.Models;
using WirtConfer.Models.States;
using WirtConfer.ViewModels;

namespace WirtConfer.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;
        private IFileManager _fileManager;
        private ISaveRepository _saveRepository;

        public EventController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IFileManager fileManager,
            ISaveRepository saveRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _fileManager = fileManager;
            _saveRepository = saveRepository;
        }


        [HttpGet]
        public IActionResult CreateEv() => PartialView(new EventViewModel());

        [HttpPost]
        public async Task<IActionResult> CreateEv(EventViewModel ev)
        {
            var curUsr = await _userManager.GetUserAsync(this.User);
            var Event = new Event_
            {
                Name = ev.Name,
                OwnerId = curUsr.Id,
                Image = await _fileManager.SaveImage(ev.Image),
            };
            await _dbContext.Events.AddAsync(Event);

            if (await _saveRepository.SaveAsync())
            {
                int id = Event.Id;
                return RedirectToAction("Event", "Event", new { id = id});
            }
            return View(ev);
        }

        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.')+1);

            return new FileStreamResult(_fileManager.ImageStream(image),$"image/{mime}");
        }

        [HttpGet]
        public IActionResult EventList()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var userInEvents = _dbContext.UserInEvents.Include(o => o.Event).Include(o => o.User).Where(o => o.User.Id == userId).ToList();
            var Own = _dbContext.Events.Where(o => o.OwnerId == userId).ToList();
            var Events = ExtractEvents(userInEvents).Union(Own);
            return View(Events); //Выводить ивенты в которых этот человек участвует или владеет             
        }

        private IEnumerable<Event_> ExtractEvents(List<UserInEvent> userInEvent)
        {
            foreach (var ev in userInEvent)
                yield return ev.Event;
        }


        public async Task<IActionResult> DeleteEvAsync(int id)
        {
            var ev = _dbContext.Events.FirstOrDefault(o => o.Id == id);
            _dbContext.Events.Remove(ev);

            if (await _saveRepository.SaveAsync())
                return RedirectToAction("EventList", "Event");
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EventAsync(int id)
        {

            var ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == id);
            return View(ev);
        }
     
    }
}
