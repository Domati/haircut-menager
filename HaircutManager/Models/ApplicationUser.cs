using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HaircutManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<OldPassword> PasswordHistory { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = new ClaimsIdentity(await manager.GetClaimsAsync(this), "ApplicationCookie");
            userIdentity.AddClaim(new Claim("NeedPasswordChange", "true"));
            return userIdentity;
        }
    }
}
