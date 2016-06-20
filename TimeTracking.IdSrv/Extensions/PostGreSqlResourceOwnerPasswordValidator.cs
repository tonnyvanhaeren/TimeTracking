using IdentityServer4.Core.Validation;
using System.Threading.Tasks;
using TimeTracking.DataAccess.Interfaces;

namespace TimeTracking.IdSrv.Extensions
{
    public class PostGreSqlResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IPostGreSqlService _service;

        public PostGreSqlResourceOwnerPasswordValidator(IPostGreSqlService service)
        {
            _service = service;
        }

        public Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            var user = _service.GetAppUserByEmail(userName); //username = email

            if (_service.VerifyAppUserPassword(user, password))
            {
                return Task.FromResult(new CustomGrantValidationResult(user.Subject, "password"));
            }

            return Task.FromResult(new CustomGrantValidationResult("Wrong username or password"));
        }
    }
}
