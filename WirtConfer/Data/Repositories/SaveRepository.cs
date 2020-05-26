using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WirtConfer.Data.Repositories
{
    public class SaveRepository : Controller, ISaveRepository
    {
        private ApplicationDbContext _dbContext;

        public SaveRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> RedirectToEvent(int idEv)
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
