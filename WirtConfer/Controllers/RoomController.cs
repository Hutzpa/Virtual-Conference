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
using WirtConfer.ViewModels;

namespace WirtConfer.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;
        private ISaveRepository _saveRepository;

        public RoomController(UserManager<User> userManager,
            ApplicationDbContext dbContext,
            ISaveRepository saveRepository)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _saveRepository = saveRepository;
        }




        public async Task<IActionResult> Room(int idRoom)
        {
            var Room = await _dbContext.Rooms.Include(o => o.Event).FirstOrDefaultAsync(o => o.Id == idRoom);
            var User = await _userManager.GetUserAsync(this.User);
            return View(new RoomViewModel { UserName = User.Name, UserSurname = User.Surname, IdEvent = Room.Event.Id, IdRoom = Room.Id });
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoomAsync(int idev, int id = 0)
        {
            if (id == 0)
                return View(new RoomViewModel { IdEvent = idev });
            var roomToChange = await _dbContext.Rooms.FirstOrDefaultAsync(o => o.Id == id);
            RoomViewModel room = new RoomViewModel
            {
                IdEvent = roomToChange.EventId,
                RoomName = roomToChange.Name,
                IdRoom = roomToChange.Id,

            };
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomViewModel rvm, int idRm)
        {
            if (!ModelState.IsValid)
                return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateRoom", rvm) });

            if (idRm == 0)
            {
                var Ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == rvm.IdEvent);
                var room = new Room
                {
                    Event = Ev,
                    Name = rvm.RoomName
                };
                await _dbContext.Rooms.AddAsync(room);
                return await _saveRepository.RedirectToEvent(rvm.IdEvent);   

            }
            else
            {
                var Ev = await _dbContext.Events.FirstOrDefaultAsync(o => o.Id == rvm.IdEvent);
                var roomToUpdate = await _dbContext.Rooms.FirstOrDefaultAsync(o => o.Id == idRm);
                roomToUpdate.Name = rvm.RoomName;


                _dbContext.Rooms.Update(roomToUpdate);
                return await _saveRepository.RedirectToEvent(rvm.IdEvent);

            }
        }

        public async Task<IActionResult> DeleteRoomAsync(int id)
        {
            var room = _dbContext.Rooms.Include(o => o.Event).FirstOrDefault(o => o.Id == id);
            _dbContext.Rooms.Remove(room);
            return await _saveRepository.RedirectToEvent(room.Event.Id);
        }

    }
}
