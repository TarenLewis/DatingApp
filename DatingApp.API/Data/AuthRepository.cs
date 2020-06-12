using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    // Responsible for querying db via entity Framework. Inject Data Context here
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;

        }

        // Controller calls this method.
        // use 'username' to id user and store in variable, and use 'password' to 
        // compute hash that 'password' generates, and compare it with the 
        // hash of the password for the username stored in the db. 
        public async Task<User> Login(string username, string password)
        {
            // Checks db for existence of user and stores user where the username given 
            // to the task matches username located in the database or returns null if
            // user input does not exist in the db.
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            // If user inputted does not exist, return null. The controller returns
            // 401 unauthorized if user is null.
            if(user == null)
                return null;

            // If the hash of 'password' does not match the hash of user.password, return 
            // 401 unauthorized
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // Successful hash comparison
            return user;
        }

            // This method compares the hash associated with the username and password given,
            // with the hash for the password stored in the db.
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                // computes hash for the byte array created by the password provided.
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Iterate and compare the hash provided by the login attempt with the 
                // hash stored in the db for that associated username.
                for(int i = 0; i < computedHash.Length; i++){
                    // Return false for unequal hashes.
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }        


        }

        // Takes user model (entity) and password to register 'password' and new user 
        // in db. 
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Add user
            await _context.Users.AddAsync(user);
            // Save changes to db
            await _context.SaveChangesAsync();

            return user;
        }

        // passwordHash and Salt, are args as reference So that when they are updated
        // inside createPasswordHash, they will also be updated in their initializations.
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // hmac = hash-based message authentication code
            // This using statement ensures that anything within the braces is dispose()' ed
            // when we are finished with it
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                // Use this key to "unlock" hashed password, and use password salt to unlock password
                // hash. Then generate equivalent hash when login user, and compare computed hash in that password
                // to computed hash stored in the database.
                passwordSalt = hmac.Key;
                //This creates a hash based on a byte array created by the string 'password'
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        // Checks if the username inputted into the system exists within the db.
        async public Task<bool> UserExists(string username)
        {
            // Username inputted exists, return true
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;
            // Username inputted does not exist, return false
            return false;
        }
    }
}