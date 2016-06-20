using System.Collections.Generic;
using TimeTracking.General.Models;

namespace TimeTracking.DataAccess.Interfaces
{
    public interface IPostGreSqlService
    {
        void AddAppUser(AppUser appUser, string password);

        bool VerifyAppUserPassword(AppUser appUser, string plainPassword);

        bool VerifyAppUserPasswordByMail(string email, string plainPassword);

        AppUser GetAppUserByEmail(string email);

        bool AppUserWithEmailIsUnique(string email);

        AppUser GetAppUserBySubject(string subject);

        void UpdateAppUser(AppUser appUser);

        void AddPolicyToAppUser(AppUser appUser, string type, string name);

        List<AppUser> GetAllAppUsers();

        List<AppUserPolicy> GetAllAppUserPolicies(string subject);

    }
}
