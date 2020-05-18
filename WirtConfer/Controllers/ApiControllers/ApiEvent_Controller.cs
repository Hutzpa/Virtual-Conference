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
        [HttpGet("{userEmail}")]
        public async Task<IEnumerable<Event_>> GetEvents(string userEmail)
        {
            var owner =await _context.Users.FirstOrDefaultAsync(o => o.Email == userEmail);
            var userEvents = _context.Events.Where(o => o.OwnerId == owner.Id);
            return userEvents;
        }

        // GET: api/ApiEvent_/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event_>> GetEvent_(int id)
        {
            var event_ = await _context.Events.Include(o=>o.Rooms).FirstOrDefaultAsync(o=>o.Id == id);

            if (event_ == null)
            {
                return NotFound();
            }

            return event_;
        }

        // POST: api/ApiEvent_
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<bool> PostEvent_Async(Event_ event_)
        {
            _context.Events.Add(event_);
            int res = await _context.SaveChangesAsync();

            return res == 1;
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
