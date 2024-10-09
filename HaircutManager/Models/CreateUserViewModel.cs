using Microsoft.AspNetCore.Mvc;

namespace HaircutManager.Models
{
    public class CreateUserViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }

}
