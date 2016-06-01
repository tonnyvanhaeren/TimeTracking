using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracking.General.Models
{
    public class UserClaim
    {
        public UserClaim(string sub)
        {
            Subject = sub;
        }

        public string Subject { get; set; }

        public int Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public string ClaimIssuer { get; set; }

        public string ClaimOriginalIssuer { get; set; }

        public override string ToString()
        {
            return $"{ClaimType} - {ClaimValue} --- {ClaimIssuer} --- {Subject}";
        }
    }
}
