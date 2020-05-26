using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WirtConfer.Data;
using WirtConfer.Data.FileManager;
using WirtConfer.Models;
using WirtConfer.Models.States;

namespace WirtConfer.Controllers
{
    [Authorize]
    public class InviteController : Controller
    {

        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;


        public InviteController(UserManager<User> userManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

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


        private string GenerateInvite(string idEv) => new StringBuilder(idEv + DateTime.Now.ToString()).GetHashCode().ToString();

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
