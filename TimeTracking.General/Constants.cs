using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracking.General
{
    public static class Constants
    {
        public static class MvcClient
        {
            public const string ClientUrl = "http://localhost:50000";
            public const string ClientUrlLogOffEndPoint = ClientUrl + "/Account/logOffMsg";
            public const string ClientUrlRegisterEndPoint = ClientUrl + "/Account/Register";
            public const string ClientEndPoint = ClientUrl + "/signin-oidc";
            public const string ClientForbiddenUrl = "/Account/Forbidden/";
        }

        public static class Idsrv
        {
            public const string IdSrvUrl = "http://localhost:60000/";
            public const string IdSrvLogOutUrl = IdSrvUrl + "ui/ExternalLogOut";
        }


        public static class AppUserPolicyType
        {
            public const string Role = "role";
            public const string Regular = "regular";
        }

        public static class AppUserPolicyRole
        {
            public const string Admin = "admin";
            public const string Employee = "employee";
        }

        public static class RoutePaths
        {
            public const string Register = "ui/register";
            public const string ExternalLogOut = "ui/externalLogout";
            //public const string Consent = "ui/consent";
            //public const string CspReport = "csp/report";
            //public const string Error = "ui/error";
            //public const string Login = "ui/login";
            //public const string Logout = "ui/logout";
            //public static readonly string[] CorsPaths;
        }

        public static class RouteNames
        {
            //public const string Register = "idsrv.authentication.register";
            //public const string ClientPermissions = "idsrv.permissions";
            //public const string CspReport = "idsrv.csp.report";
            //public const string Login = "idsrv.authentication.login";
            //public const string LoginExternal = "idsrv.authentication.loginexternal";
            //public const string LoginExternalCallback = "idsrv.authentication.loginexternalcallback";
            //public const string Logout = "idsrv.authentication.logout";
            //public const string LogoutPrompt = "idsrv.authentication.logoutprompt";
            //public const string ResumeLoginFromRedirect = "idsrv.authentication.resume";
            //public const string Welcome = "idsrv.welcome";
        }

        public static class Provider
        {
            public const string Name = "self";
            public const string Id = "selfId";
        }
    }
}
