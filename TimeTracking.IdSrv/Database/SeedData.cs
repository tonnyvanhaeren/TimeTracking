using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TimeTracking.DataAccess;
using TimeTracking.General.Helpers;
using TimeTracking.General.Models;
namespace TimeTracking.IdSrv.Database
{
    public static class SeedData
    {

        public static void CreateAdminUser(IServiceProvider serviceProvider, IConfigurationRoot configuration)
        {
            var _passwordHasher = serviceProvider.GetService<PasswordHasher>();
            var context = serviceProvider.GetService<PostGreSqlDbContext>();

            var _service = new PostGreSqlService(context, _passwordHasher);

            //getting admin user settings out of json (user secrets) config files
            //check if a user with mail exists (email is index unique)
            //if not create admin user
            if (!_service.UserWithEmailExists(configuration["Admin:Email"]))
            {
                var user = new User { Email = configuration["Admin:Email"],
                                      GivenName = configuration["Admin:GivenName"],
                                      FamilyName = configuration["Admin:FamilyName"],
                                      EmailConfirmed = true,
                                      Enabled = true};

                _service.AddUser(user, configuration["Admin:Password"]);

            }

            //var retUser = _service.GetUserByEmail(configuration["Admin:Email"]);
            //bool ok = _passwordHasher.VerifyHashedPassword(retUser.Password, configuration["Admin:Password"]);

            //cleanUp
            _passwordHasher = null;
            _service = null;
            context = null;
            
        }
    }
}

