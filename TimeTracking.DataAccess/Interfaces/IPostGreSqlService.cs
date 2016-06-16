using TimeTracking.General.Models;

namespace TimeTracking.DataAccess.Interfaces
{
    public interface IPostGreSqlService
    {
        void AddAppUser(AppUser appUser, string password);

        AppUser GetAppUserByEmail(string email);

        bool AppUserWithEmailIsUnique(string email);

        AppUser GetAppUserBySubject(string subject);

        void UpdateAppUser(AppUser appUser);

        void AddPolicyToAppUser(AppUser appUser, string type, string name);

    }
}
