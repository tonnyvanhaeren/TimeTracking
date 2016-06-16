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
        /// <param name="AppUser"></param>
        /// <param name="plainPassword"></param>
        public void AddAppUser(AppUser appUser, string plainPassword)
        {
            //hash user password
            appUser.Password = HashPassword(plainPassword);
            
            _context.AppUsers.Add(appUser);
            _context.SaveChangesAsync();
        }

        public AppUser GetAppUserByEmail(string email)
        {
            return _context.AppUsers.FirstOrDefault<AppUser>(u => u.Email == email);

        }

        public AppUser GetAppUserBySubject(string subject)
        {
            return _context.AppUsers.FirstOrDefault<AppUser>(u => u.Subject == subject);
        }

        public void UpdateAppUser(AppUser appUser)
        {
            _context.Update<AppUser>(appUser);
            _context.SaveChanges();
        }

        public bool AppUserWithEmailIsUnique(string email)
        {
            if (GetAppUserByEmail(email) == null)
            {
                return true;
            }
            return false;
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
        private bool VerifyPassword(AppUser appUser, string plainPassword)
        {
            return _passwordHasher.VerifyHashedPassword(appUser.Password, plainPassword);
        }

        public void AddPolicyToAppUser(AppUser appUser, string type, string name)
        {
            var policy = new AppUserPolicy(appUser.Subject);
            policy.Type = type;
            policy.Name = name;

            _context.AppUserPolicies.Add(policy);
            _context.SaveChanges();
        }
    }
}
