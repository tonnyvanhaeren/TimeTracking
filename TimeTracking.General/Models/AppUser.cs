using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracking.General.Models
{
    public class AppUser
    {

        public AppUser()
        {
            EmailConfirmed = false;
            Enabled = false;
            Provider = Constants.Provider.Name;
            ProviderId = Constants.Provider.Id;
            Subject = Guid.NewGuid().ToString();
            AppUserPolicies = new List<AppUserPolicy>();
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

        public List<AppUserPolicy> AppUserPolicies { get; set; }
    }
}
