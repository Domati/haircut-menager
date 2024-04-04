using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace HaircutManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
