using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Data;
using WirtConfer.Models;
using WirtConfer.Models.States;
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

        /// <summary>
        /// //////////////////////////////////////////////EVENTS
        /// </summary>
        /// <returns></returns>
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
            
            if (await SaveAsync())
                return RedirectToAction("EventList", "Event");

            return View(evm);

        }

        [HttpGet]
        public IActionResult EventList(string UserId = null)
        {
            var userId = UserId ?? _userManager.GetUserId(HttpContext.User);
            return View(_dbContext.Events.Where(o => o.OwnerId == userId).ToList()); //Выводить ивенты в которых этот человек участвует или владеет 
        }

        //[HttpGet]
        //public IActionResult EventList() => View();

        [HttpPost]
        public async Task<IActionResult> DeleteEvAsync(int id)
        {
            _dbContext.Events.Remove(_dbContext.Events.FirstOrDefault(o => o.Id == id));
            
            if (await SaveAsync())
                return RedirectToAction("EventList", "Event");
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EventAsync(int id)
        {
            var ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == id);
            return View(new EventViewModel { IdEvent = ev.Id, Name = ev.Name });
        }


        /// <summary>
        /// /////////////////////////////////////////// ROOOOOOMS 
        /// </summary>
        /// <param name="idRoom"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> Room(int idRoom)
        {
            var Room = await _dbContext.Rooms.Include(o => o.Event).FirstOrDefaultAsync(o => o.Id == idRoom);
            var User = await _userManager.GetUserAsync(this.User);
            return View(new RoomViewModel { UserName = User.Name, UserSurname = User.Surname, IdEvent = Room.Event.Id, IdRoom = Room.Id });
        }



        [HttpGet]
        public IActionResult CreateRoom(int idEv) => View(new RoomViewModel { IdEvent = idEv });

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomViewModel rvm)
        {
            var Ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == rvm.IdEvent);
            var room = new Room
            {
                Event = Ev,
                Name = rvm.RoomName
            };
            await _dbContext.Rooms.AddAsync(room);
            
            if (await SaveAsync())
                return View(rvm);
            
            return RedirectToAction("Event", "Event", new {id = rvm.IdEvent });
        }

        public async Task<IActionResult> DeleteRoomAsync(int id)
        {
            var room = _dbContext.Rooms.Include(o=>o.Event).FirstOrDefault(o => o.Id == id);
            _dbContext.Rooms.Remove(room);
            return await RedirectToEvent(room.Event.Id);
        }

        /// <summary>
        /// ///////////////////////////////////////////////INVITES
        /// </summary>
        /// <param name="idEv"></param>
        /// <param name="invType"></param>
        /// <returns></returns>

        public async Task<IActionResult> CreateInviteAsync(int idEv,int invType)
        {
            var Invite = new Invite
            {
                Url = GenerateInvite(idEv.ToString()),
                Type = (InviteType)invType,
                Event = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == idEv),
            };
            _dbContext.Invites.Add(Invite);
            return await RedirectToEvent(idEv);
        }
        private string GenerateInvite(string idEv) => new StringBuilder(idEv + DateTime.Now).GetHashCode().ToString();

        public async Task<IActionResult> DeleteInviteAsync(int idInv)
        {
            var inv = await _dbContext.Invites.FirstOrDefaultAsync(o => o.Id == idInv);
            _dbContext.Invites.Remove(inv);
            return await RedirectToEvent(inv.EventId);
        }

        public async Task<bool> SaveAsync()
        {
            var saveRes = await _dbContext.SaveChangesAsync();
            return saveRes != 0;
        }


        [HttpGet]
        public async Task<IActionResult> JoinInviteAsync(string Url)
        {
            var askedInvite = _dbContext.Invites.Where(o => o.Url == Url);
            if (askedInvite.Count() == 0)
                return RedirectToAction("Index", "Home");
            Invite inv = askedInvite.First();
            var u = _userManager.GetUserId(HttpContext.User);
            var blacklist = _dbContext.Blacklist.Include(o => o.User).Where(o => o.User.Id == u); //Проверять, не в блек листе ли пользователь

                var User = await _userManager.GetUserAsync(this.User);
                var Event = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == inv.EventId);
                UserInEvent uie = new UserInEvent
                {
                    User = User,
                    Event = Event,
                };
            _dbContext.UserInEvents.Add(uie);
            if (inv.Type == InviteType.Single)
                _dbContext.Invites.Remove(inv);

            return await RedirectToEvent(inv.EventId);
        }

        private async Task<IActionResult> RedirectToEvent(int idEv)
        {
            if (await SaveAsync())
                return RedirectToAction("Event", "Event", new { id = idEv });
            else
                return RedirectToAction("Index", "Home");
        }

    }
}
