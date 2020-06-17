using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WirtConfer.Data;
using WirtConfer.Models;

namespace WirtConfer.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiEvent_Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiEvent_Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiEvent_/email@gmail.com
        [HttpGet]
        public async Task<IEnumerable<Event_>> GetEvents(string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(o => o.Email == userEmail); 
            var userInEvents = _context.UserInEvents.Include(o => o.Event).Include(o => o.User).Where(o => o.User.Id == user.Id && o.IsBanned == false).ToList();
            var Own = _context.Events.Where(o => o.OwnerId == user.Id).ToList();
            var Events = ExtractEvents(userInEvents).Union(Own);
            return Events;
        }

        private IEnumerable<Event_> ExtractEvents(List<UserInEvent> userInEvent)
        {
            foreach (var ev in userInEvent)
                yield return ev.Event;
        }

        // POST: api/ApiEvent_
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task PostEvent_Async([FromBody]Event_ event_)
        {
            User user = await _context.Users.FirstOrDefaultAsync(o => o.Email == event_.OwnerId);
            event_.OwnerId = user.Id;
            _context.Events.Add(event_);
            await _context.SaveChangesAsync();
        }

        // DELETE: api/ApiEvent_/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Event_>> DeleteEvent_(int id)
        {
            var event_ = await _context.Events.FindAsync(id);
            if (event_ == null)
            {
                return NotFound();
            }

            _context.Events.Remove(event_);
            await _context.SaveChangesAsync();

            return event_;
        }

      
    }
}
