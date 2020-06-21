using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {


            // user: taren password: pwd
            // (used for testing purposes) throw new Exception("new exception thrown! (in AuthController.cs, Login method.)");

            // Checking to make sure that we have a user and the username
            // and pass matches what's stored in the Db for that particular user.
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            // THe user is not found in the database, however,
            // This does not allow the login attempt any glimpse at a "hint",
            // eg. Username exists but password is wrong, or username doesn't exist, etc.
            if (userFromRepo == null)
                return Unauthorized();


            // Build a token which gets returned to user. Contains user ID and Username
            var claims = new[]
            {
                // ID Token claim
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                // Username Token Claim
                new Claim(ClaimTypes.Name, userFromRepo.Username)
                };

            // To ensure the token is a valid token when it comes back, the server
            // needs a key to sign this token. This creates and encrypts a security 
            // key and using the key as part of the signing credentials.
            // This AppSettings:Token in reality should not be shor t, it should
            // be an extremely long randomly generated token. The token in this project
            // is very short and simple for the purposes of this exersize.
            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Generate Signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create security token descriptor which contains claims, 
            // expiry date for token, and sign in credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // expy of 1 day is for the purposes of training. Something like
                // a bank might have an expiration date of 30 minutes, or something
                // less strict might be permant until logout, etc. etc.
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Security token handler allows us to create the token based on the 
            // token handler which is given the token descriptor being passed,
            // and is then stored in token variable.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Use token variable to write token into response which is sent
            // back to the client.
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        } // end try

    }

}


