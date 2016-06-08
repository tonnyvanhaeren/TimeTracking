using TimeTracking.General.Models;

namespace TimeTracking.DataAccess.Interfaces
{
    public interface IPostGreSqlService
    {
        void AddUser(User user, string password);

        User GetUserByEmail(string email);

        bool UserWithEmailIsUnique(string email);

        User GetUserBySubject(string subject);

        void UpdateUser(User user);
    }
}
