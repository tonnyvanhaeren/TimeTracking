using TimeTracking.General.Models;

namespace TimeTracking.DataAccess.Interfaces
{
    interface IPostGreSqlService
    {
        void AddUser(User user, string password);

        User GetUserByEmail(string email);

        bool UserWithEmailExists(User user);

        User GetUserBySubject(string subject);
    }
}
