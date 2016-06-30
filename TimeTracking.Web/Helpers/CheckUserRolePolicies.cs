using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TimeTracking.Web.Helpers
{
    public static class CheckUserRolePolicies
    {
        public static bool HasRolePolicy(ClaimsPrincipal user, string RoleName)
        {
            return user.HasClaim(c => c.Type == JwtClaimTypes.Role && c.Value == RoleName);
        }
    }
}
