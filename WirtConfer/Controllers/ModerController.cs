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
using WirtConfer.Data.Repositories;
using WirtConfer.Models;
using WirtConfer.Models.States;

namespace WirtConfer.Controllers
{
    [Authorize]
    public class ModerController : Controller
    {

        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;
        private ISaveRepository _saveRepository;

        public ModerController(UserManager<User> userManager,
            ApplicationDbContext dbContext,
            ISaveRepository saveRepository)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _saveRepository = saveRepository;
        }


        public async Task<IActionResult> MakeModerAsync(string id, int evId) => await ChangeModeratorAsync(Roles.moderator, id, evId);
        public async Task<IActionResult> DeleteModerAsync(string id, int evId) => await ChangeModeratorAsync(Roles.regularUser, id, evId);
        private async Task<IActionResult> ChangeModeratorAsync(Roles role, string id, int evId)
        {
            UserInEvent uie = await _dbContext.UserInEvents.Include(o => o.Event).Include(o => o.User).FirstOrDefaultAsync(o => o.User.Id == id && o.Event.Id == evId); 
            uie.Role = role;
            _dbContext.UserInEvents.Update(uie);
            return await _saveRepository.RedirectToEvent(uie.Event.Id);
        }

        public async Task<IActionResult> LeaveAsync(int idEv)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            UserInEvent uie = await _dbContext.UserInEvents.Include(o => o.User).Include(o => o.Event).FirstOrDefaultAsync(o => o.Event.Id == idEv && o.User.Id == userId);

            _dbContext.UserInEvents.Remove(uie);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("EventList", "Event");
        }

        public async Task<IActionResult> Ban(string id, int evId)
        {
            var curUsr = await _userManager.GetUserAsync(this.User);
            UserInEvent current = await _dbContext.UserInEvents.Include(o => o.Event).Include(o => o.User).FirstOrDefaultAsync(o => o.User.Id == _userManager.GetUserId(HttpContext.User) && o.Event.Id == evId);
            if(current.Role != Roles.moderator)
                return RedirectToAction("Event", "Event", new { id = evId });


            UserInEvent uie = await _dbContext.UserInEvents.Include(o => o.Event).Include(o => o.User).FirstOrDefaultAsync(o => o.User.Id == id && o.Event.Id == evId);
            _dbContext.UserInEvents.Remove(uie);
            Blacklist blacklist = new Blacklist
            {
                IdUser = uie.User.Id,
                User = uie.User,
                Event = uie.Event,
                IdEvent = uie.Event.Id,

            };
            _dbContext.Blacklist.Add(blacklist);
            return await _saveRepository.RedirectToEvent(uie.Event.Id);
        }
    }
}
