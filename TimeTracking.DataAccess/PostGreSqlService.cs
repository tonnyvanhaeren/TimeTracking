using System;
using System.Linq;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Helpers;
using TimeTracking.General.Models;

namespace TimeTracking.DataAccess
{
    public class PostGreSqlService : IPostGreSqlService
    {
        private readonly PostGreSqlDbContext _context;
        private readonly PasswordHasher _passwordHasher;

        public PostGreSqlService(PostGreSqlDbContext context, PasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Add-Save user and hash password automacivly
        /// </summary>
        /// <param name="user"></param>
        /// <param name="plainPassword"></param>
        public void AddUser(User user, string plainPassword)
        {
            //hash user password
            user.Password = HashPassword(plainPassword);
            
            _context.Users.Add(user);
            _context.SaveChangesAsync();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault<User>(u => u.Email == email);

        }

        public User GetUserBySubject(string subject)
        {
            return _context.Users.FirstOrDefault<User>(u => u.Subject == subject);
        }

        public bool UserWithEmailExists(string email)
        {
            if (GetUserByEmail(email) == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Hash user password
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <returns>string hashed password</returns>
        private string HashPassword(string plainPassword)
        {
            return _passwordHasher.HashPassword(plainPassword);
        }


        /// <summary>
        /// verify given password with user persistent hashed password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        private bool VerifyPassword(User user, string plainPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user.Password, plainPassword);
        }

    }
}
