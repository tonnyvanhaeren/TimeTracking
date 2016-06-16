using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracking.General.Models
{
    public class AppUserPolicy
    {
        public AppUserPolicy(string sub)
        {
            this.Subject = sub;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Subject { get; set; }
    }
}
