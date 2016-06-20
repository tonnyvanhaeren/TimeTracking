using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracking.General.Models;

namespace TimeTracking.Web.Views.Account
{
    public class AppUserViewModelView
    {
        public AppUserViewModelView(AppUser appUser, List<AppUserPolicy> policies)
        {
            AppUser = appUser;
            AppUserPolicies = policies;
        }

        public AppUser AppUser { get; set; }

        public List<AppUserPolicy> AppUserPolicies { get; set; }

    }
}
