using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using WirtConfer.Data;
using WirtConfer.Models;
using WirtConfer.Models.States;

namespace WirtConfer.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiInvite_Controller : ControllerBase
    {
        private ApplicationDbContext _dbContext;

        public ApiInvite_Controller(ApplicationDbContext context)
        {
            _dbContext = context;
        }
        

        // GET: api/ApiInvite_/5
        [HttpGet]
        public async Task<bool> GetAsync(string email, string url)
        {

            //Проверка инвайта на существование, получение в случае елси существует
            var askedInvite = _dbContext.Invites.Include(o => o.Event).Where(o => o.Url == url);
            if (askedInvite.Count() == 0)
                return false;
            Invite inv = askedInvite.First();

            //Не в блеклисте ли пользователь
            var User = await _dbContext.Users.FirstOrDefaultAsync(o => o.Email == email);
            var userId = User.Id;
            var blacklist = _dbContext.Blacklist.Include(o => o.User).Include(o => o.Event).ToList().Exists(o => o.User.Id == userId && o.Event.Id == inv.Event.Id);
            if (blacklist)
                return false;

            //Нет ли этого пользователя уже в этом ивенте
            var thisUserInThisEvent = _dbContext.UserInEvents.Include(o => o.User).Include(o => o.Event).ToList().Exists(o => o.Event.Id == inv.Event.Id && o.User.Id == userId);
            if (thisUserInThisEvent)
                return false;

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

            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}
