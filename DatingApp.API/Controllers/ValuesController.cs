using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    // values controller example request
    // http:localhost:5000/api/values 
    [Route("api/[controller]")]

    // 1. Enforces attribute routing (located above), rather than conventional routing
    // 2. Automatically validates the request 
    [ApiController]
    
    // 'ControllerBase' gives access to http responses, and actions which can be used
    // in the controller. ('ControllerBase' does NOT have view support. 'Controller' HAS view support)
    public class ValuesController : ControllerBase
    {
        // Because this is a private field, it is standard practice to
        // include a '_' character in front of the field name.
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            _context = context;

        }

        // GET api/values
        // IEnumerable is a collection of "things", in this case a collection
        // of strings
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            // Go to DB, get Values as a list, and store them in 'values'
            // with an HTTP 200 OK response. 
            var values = await _context.Values.ToListAsync();

            return Ok(values);
        }

        //This allow anonymous is for the purposes of experimentation
        // to show the difference between an authorized request handlin
        // and anonymous request being allowed. This function will
        // be allowed to be received without a token.
        [AllowAnonymous]
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            // FirstOrDefault returns default null value if an id is not found,
            // in order to avoid an expensive exception.
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        // (used for creating a record)
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        // (used for editing a record)
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        // (used for deleting a record)
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}