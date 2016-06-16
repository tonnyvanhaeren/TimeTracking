using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using TimeTracking.General.Models;

namespace TimeTracking.General.Helpers
{
    public class ConfirmationToken
    {
        private readonly IDataProtector _protector;

        public ConfirmationToken(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("TimeTracking.General.Token");
        }

        public string Generate(string purpose,  AppUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            
            sw.WriteLine(DateTimeOffset.UtcNow);
            sw.WriteLine(user.Subject);
            sw.WriteLine(purpose ?? "");
            sw.WriteLine(user.SecurityStamp);
            sw.Flush();

            var protectedBytes = _protector.Protect(ms.ToArray());
            return Convert.ToBase64String(protectedBytes);
        }

        public bool ValidateToken(string purpose, string token, AppUser user)
        {

            var unprotectedData = _protector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);

            var sr = new StreamReader(ms);

            var creationTime = sr.ReadLine();

            var offsetTime = DateTimeOffset.Parse(creationTime);

            var timeSpan = new TimeSpan(0, 10, 0);
            var expirationTime = offsetTime + timeSpan ;

            //controle of mail respons is between 10 min
            if (expirationTime < DateTimeOffset.UtcNow)
            {
                return false;
            }

            var userId = sr.ReadLine();
 
            if (userId != user.Subject)
            {
                return false;
            }

            var purp = sr.ReadLine();

            if (!string.Equals(purp, purpose))
            {
                return false;
            }

            var stamp = sr.ReadLine();

            if (stamp == user.SecurityStamp) { 
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
