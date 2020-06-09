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

        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;
        private IFileManager _fileManager;
        private ISaveRepository _saveRepository;

        public EventController(UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IFileManager fileManager,
            ISaveRepository saveRepository)
        {

            _userManager = userManager;
            _dbContext = dbContext;
            _fileManager = fileManager;
            _saveRepository = saveRepository;
        }


        [HttpGet]
        public async Task<IActionResult> CreateEvAsync(int id = 0)
        {

            if (id == 0)
                return View(new EventViewModel());

            var eventToChange = await _dbContext.Events.FindAsync(id);
            if (eventToChange == null)
                return NotFound();

            EventViewModel ev = new EventViewModel
            {
                IdEvent = eventToChange.Id,
                OwnerId = eventToChange.OwnerId,
                Name = eventToChange.Name,
            };
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEv(EventViewModel ev, int idEv)
        {
            if (!ModelState.IsValid)
                return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateEv", ev) });
            //if (!ModelState.IsValid)
            //    return View(ev);

            if (idEv == 0)
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
                    return RedirectToAction("Event", "Event", new { id = id });
                }
                return View(ev);
            }
            else
            {
                var toUpdate = _dbContext.Events.FirstOrDefault(o => o.Id == idEv);
                toUpdate.Image = await _fileManager.SaveImage(ev.Image);
                toUpdate.Name = ev.Name;
                _dbContext.Events.Update(toUpdate);
                if (await _saveRepository.SaveAsync())
                {
                    int id = toUpdate.Id;
                    return RedirectToAction("Event", "Event", new { id = id });
                }
            }
            return View(ev);
        }

        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);

            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
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
            string curUsrId = _userManager.GetUserId(HttpContext.User);
            var thisUserInThisEvent = _dbContext.UserInEvents.Include(o => o.User).Include(o => o.Event).ToList().Exists(o => o.Event.Id == id && o.User.Id == curUsrId);
            var ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == id);
            if (!thisUserInThisEvent && ev.OwnerId != curUsrId) //Если пользователь не состоит в ивенте и не владеет им
                return RedirectToAction("Index", "Home");

            return View(ev);
        }

    }
}
