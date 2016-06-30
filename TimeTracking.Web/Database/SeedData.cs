using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TimeTracking.DataAccess;
using TimeTracking.General;
using TimeTracking.General.Helpers;
using TimeTracking.General.Models;

namespace TimeTracking.Web
{
    public static class SeedData
    {

        public static void CreateAdminAppUser(IServiceProvider serviceProvider, IConfigurationRoot configuration)
        {
            //todo: clean up DI 
            var _passwordHasher = serviceProvider.GetService<PasswordHasher>();
            var context = serviceProvider.GetService<PostGreSqlDbContext>();
            var _service = new PostGreSqlService(context, _passwordHasher);

            //get admin user settings out of json (user secrets) config files
            //if a admin user with mail exists (email index unique) do not create!! 
            if (_service.AppUserWithEmailIsUnique(configuration["Admin:Email"]))
            {
                var appUser = new AppUser { Email = configuration["Admin:Email"],
                                            GivenName = configuration["Admin:GivenName"],
                                            FamilyName = configuration["Admin:FamilyName"],
                                            Username = configuration["Admin:Email"],
                                            EmailConfirmed = true,
                                            Enabled = true};

                _service.AddAppUser(appUser, configuration["Admin:Password"]);
                _service.AddPolicyToAppUser(appUser, Constants.AppUserPolicyType.Role, Constants.AppUserPolicyRole.Admin);
                _service.AddPolicyToAppUser(appUser, Constants.AppUserPolicyType.Role, Constants.AppUserPolicyRole.Employee);
            }

            var retUser = _service.GetAppUserByEmail(configuration["Admin:Email"]);

            bool ok = _passwordHasher.VerifyHashedPassword(retUser.Password, configuration["Admin:Password"]);

            int total = retUser.AppUserPolicies.Count;




            //cleanUp
            _passwordHasher = null;
            _service = null;
            context = null;
            
        }
    }
}

