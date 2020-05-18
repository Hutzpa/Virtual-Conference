using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using WirtConfer.Data;

namespace WirtConfer.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiInvite_Controller : ControllerBase
    {
        private ApplicationDbContext _context;

        public ApiInvite_Controller(ApplicationDbContext context)
        {
            _context = context;
        }
        

        // GET: api/ApiInvite_/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(string email, string inviteNumber)
        {
            return "value";
        }

        // POST: api/ApiInvite_
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ApiInvite_/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
