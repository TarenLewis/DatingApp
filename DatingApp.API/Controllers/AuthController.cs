using System;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    // Route replaces api/[controller] with "first" part of 
    // controller name. In this case, route is api/auth/..
    // In the case of the ValuesController, route would be
    // api/values/..
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Inject created repository.
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        // eg. localhost:5000/api/auth/register
        [HttpPost("register")]
        // Takes username and password
        // Will be received as a JSON serialized object (Data Transfer Object).
        // The DTO will store the serialized JSON object received from the client.
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // Validate request

            // Make sure username is stored as all lowercase
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            // Ensure username given does not already exist 
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username Already Exists");

            // Creates instance of user class
            var userToCreate = new User
            {
                // This is the only information which has yet been passed.
              Username = userForRegisterDto.Username
            };

            // Registers user just created with the provided password
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            


            // Returns success message. (Should return CreatedAtRoute() function
            // in other words, location in db of newly created entity:
            // return CreatedAtRoute();
        
            // This is temporary, it is the same status code as CreatedAtRoute();
            return StatusCode(201);
        }

    }
}



// System.Console.WriteLine("\n-\n-\n-\nDEBUG:\n\n");
//             // Debug Here:
//             System.Console.WriteLine("\n-\n-\n-");