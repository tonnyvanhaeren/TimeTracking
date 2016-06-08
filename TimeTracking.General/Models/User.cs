using System;
using System.Collections.Generic;
using System.Security.Claims;
namespace TimeTracking.General.Models
{
    public class User
    {
        public User()
        {
            EmailConfirmed = false;
            Enabled = false;
            Provider = Constants.Provider.Name;
            ProviderId = Constants.Provider.Id;
            Subject = Guid.NewGuid().ToString();
            UserClaims = new List<UserClaim>();
            DateCreated = DateTime.UtcNow;
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public DateTime DateCreated { get; set; }

        public string Subject { get; set; }

        public string SecurityStamp { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime BirthDate { get; set; }

        public string Password { get; set; }

        public int Gender { get; set; }

        public string Username { get; set; }

        public bool Enabled { get; set; }

        public string Provider { get; set; }

        public string ProviderId { get; set; }

        public IEnumerable<UserClaim> UserClaims { get; set; }
    }
}
