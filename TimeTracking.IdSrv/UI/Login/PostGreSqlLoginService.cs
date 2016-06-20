using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Models;

namespace TimeTracking.IdSrv.UI.Login
{
    public class PostGreSqlLoginService
    {
        private readonly IPostGreSqlService _service;

        public PostGreSqlLoginService(IPostGreSqlService service)
        {
            _service = service;
        }

        public bool ValidateCredentials(string username, string password)
        {
            return _service.VerifyAppUserPasswordByMail(username, password);
        }

        public AppUser FindByUsername(string username)
        {
            return _service.GetAppUserByEmail(username);
        }
    }
}
