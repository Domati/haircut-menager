using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

public class RolesInitializer
{
    public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Klient", "Fryzjer" }; // Dodaj więcej ról, jeśli potrzebujesz
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Utwórz role, jeśli nie istnieją
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Tutaj możesz również dodać domyślnego użytkownika admina jak w poprzednich przykładach
    }
}
