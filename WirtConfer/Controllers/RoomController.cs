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
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private ApplicationDbContext _dbContext;
        private IFileManager _fileManager;
        private ISaveRepository _saveRepository;

        public RoomController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            ApplicationDbContext dbContext,
            IFileManager fileManager,
            ISaveRepository saveRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _fileManager = fileManager;
            _saveRepository = saveRepository;
        }



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
            return await _saveRepository.RedirectToEvent(rvm.IdEvent);
        }

        public async Task<IActionResult> DeleteRoomAsync(int id)
        {
            var room = _dbContext.Rooms.Include(o => o.Event).FirstOrDefault(o => o.Id == id);
            _dbContext.Rooms.Remove(room);
            return await _saveRepository.RedirectToEvent(room.Event.Id);
        }

    }
}
