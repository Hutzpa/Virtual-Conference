using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WirtConfer.Data.Repositories
{
    public interface ISaveRepository
    {
        Task<IActionResult> RedirectToEvent(int idEv);
        Task<bool> SaveAsync();
    }
}
