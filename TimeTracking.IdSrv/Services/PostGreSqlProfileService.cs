using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Extensions;
using System.Threading.Tasks;


using System.Collections.Generic;
using System.Security.Claims;

using IdentityModel;



using TimeTracking.DataAccess.Interfaces;

namespace TimeTracking.IdSrv.Services
{
    public class PostGreSqlProfileService : IProfileService
    {
        private readonly IPostGreSqlService _service;

        public PostGreSqlProfileService(IPostGreSqlService service)
        {
            _service = service;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = _service.GetAppUserBySubject(subjectId);

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Subject),
                new Claim(JwtClaimTypes.Name, $"{user.GivenName} {user.FamilyName}"),
                new Claim(JwtClaimTypes.GivenName, user.GivenName),
                new Claim(JwtClaimTypes.FamilyName, user.FamilyName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower(), ClaimValueTypes.Boolean)
            };


            //get user policy if any
            var policies = _service.GetAllAppUserPolicies(user.Subject);

            foreach(var pol in policies)
            {
                if (pol.Type == General.Constants.AppUserPolicyType.Role)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, pol.Name));
                }
            }


            context.IssuedClaims = claims;

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = _service.GetAppUserBySubject(context.Subject.GetSubjectId());
            context.IsActive = (user != null) && user.Enabled;
            return Task.FromResult(0);
        }
    }
}
