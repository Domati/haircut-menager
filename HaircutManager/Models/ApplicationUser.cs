using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Diagnostics.Eventing.Reader;

namespace HaircutManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<OldPassword> PasswordHistory { get; set; }
        public List<OtpInstance> OneTimePasswords { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime LastActivity { get; set; }
        public bool InactiveLogoutEnabled { get; set; }
        public TimeSpan InactiveTimeUntilLogout { get; set; } = TimeSpan.FromMinutes(30);
    }
}
