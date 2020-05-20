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
    public class ApiRooms_Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiRooms_Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiRooms_/5
        //[HttpGet("{id}")]
        [HttpGet]
        public IQueryable<Room> GetEvent_(int id)
        {
            var rooms = _context.Rooms.Include(o => o.Event).Where(o => o.Event.Id == id);
            //var rooms = _context.Rooms.Include(o => o.Event).(o => o.Event.Id == id);
            return rooms;
        }

        // POST: api/ApiRooms_
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task PostRoom(Room room)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(o => o.Id == room.EventId);
            room.EventId = 0;
            room.Event = ev;

            _context.Rooms.Add(room);
            int res = await _context.SaveChangesAsync();

        }

        // DELETE: api/ApiRooms_/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Room>> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return room;
        }

    }
}
