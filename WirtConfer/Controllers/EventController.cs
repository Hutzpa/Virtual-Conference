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

        public EventController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IFileManager fileManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _fileManager = fileManager;
        }

        /// <summary>
        /// //////////////////////////////////////////////EVENTS
        /// </summary>
        /// <returns></returns>
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

            if (await SaveAsync())
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

            if (await SaveAsync())
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

        public async Task<IActionResult> MakeModerAsync(string id, int evId) => await ChangeModeratorAsync(Roles.moderator, id,evId);
        public async Task<IActionResult> DeleteModerAsync(string id, int evId) => await ChangeModeratorAsync(Roles.regularUser, id,evId);
        private async Task<IActionResult> ChangeModeratorAsync(Roles role, string id,int evId)
        {
            UserInEvent uie = await _dbContext.UserInEvents.Include(o => o.Event).Include(o => o.User).FirstOrDefaultAsync(o => o.User.Id == id && o.Event.Id == evId); //ПОлучает неправильный id ивента 
            uie.Role = role;
            _dbContext.UserInEvents.Update(uie);
            return await RedirectToEvent(uie.Event.Id);
        }

        public async Task<IActionResult> LeaveAsync(int idEv)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            UserInEvent uie = await _dbContext.UserInEvents.Include(o => o.User).Include(o => o.Event).FirstOrDefaultAsync(o => o.Event.Id == idEv && o.User.Id == userId);

            _dbContext.UserInEvents.Remove(uie);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("EventList","Event");
        }

        public async Task<IActionResult> Ban(string id)
        {
            UserInEvent uie = await _dbContext.UserInEvents.Include(o => o.User).Include(o=>o.Event).FirstOrDefaultAsync(o => o.User.Id == id);
            _dbContext.UserInEvents.Remove(uie);
            Blacklist blacklist = new Blacklist
            {
                IdUser = uie.User.Id,
                User = uie.User,
                Event = uie.Event,
                IdEvent = uie.Event.Id,
                
            };
            _dbContext.Blacklist.Add(blacklist);
            return await RedirectToEvent(uie.Event.Id);
        }


        /// <summary>
        /// /////////////////////////////////////////// ROOOOOOMS 
        /// </summary>

        [HttpGet]
        public async Task<IActionResult> Room(int idRoom)
        {
            var Room = await _dbContext.Rooms.Include(o => o.Event).FirstOrDefaultAsync(o => o.Id == idRoom);
            
            var User = await _userManager.GetUserAsync(this.User);
            if (Room.Event.OwnerId == _userManager.GetUserId(HttpContext.User))
                return View("RoomAdmin", new RoomViewModel { UserName = User.Name, UserSurname = User.Surname, IdEvent = Room.Event.Id, IdRoom = Room.Id });
            return View(new RoomViewModel { UserName = User.Name, UserSurname = User.Surname, IdEvent = Room.Event.Id, IdRoom = Room.Id });
        }

        [HttpGet]
        public IActionResult CreateRoom(int idev) => View(new RoomViewModel { IdEvent = idev });

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
            return await RedirectToEvent(rvm.IdEvent);
        }

        public async Task<IActionResult> DeleteRoomAsync(int id)
        {
            var room = _dbContext.Rooms.Include(o => o.Event).FirstOrDefault(o => o.Id == id);
            _dbContext.Rooms.Remove(room);
            return await RedirectToEvent(room.Event.Id);
        }

        /// <summary>
        /// ///////////////////////////////////////////////INVITES
        /// </summary>
        /// <param name="idEv"></param>
        /// <param name="invType"></param>
        /// <returns></returns>

        public async Task<IActionResult> CreateInviteAsync(int idEv, int invType)
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

        [HttpGet]
        public async Task<IActionResult> JoinInviteAsync(string Url)
        {
            //Проверка инвайта на существование, получение в случае елси существует
            var askedInvite = _dbContext.Invites.Include(o => o.Event).Where(o => o.Url == Url);
            if (askedInvite.Count() == 0)
                return RedirectToAction("Index", "Home");
            Invite inv = askedInvite.First();

            //Не в блеклисте ли пользователь
            var userId = _userManager.GetUserId(HttpContext.User);  
            var blacklist = _dbContext.Blacklist.Include(o => o.User).Include(o => o.Event).ToList().Exists(o => o.User.Id == userId && o.Event.Id == inv.Event.Id);
            if (blacklist)
                return RedirectToAction("Index", "Home");

            //Нет ли этого пользователя уже в этом ивенте
            var thisUserInThisEvent = _dbContext.UserInEvents.Include(o => o.User).Include(o => o.Event).ToList().Exists(o => o.Event.Id == inv.Event.Id && o.User.Id == userId);
            if (thisUserInThisEvent)
                return RedirectToAction("Event", "Event", new { id = inv.Event.Id });

            var User = await _userManager.GetUserAsync(this.User);
            var Event = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == inv.EventId);
            UserInEvent uie = new UserInEvent
            {
                User = User,
                Event = Event,
                Role = Roles.regularUser,
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
        public async Task<bool> SaveAsync()
        {
            var saveRes = await _dbContext.SaveChangesAsync();
            return saveRes != 0;
        }
    }
}
