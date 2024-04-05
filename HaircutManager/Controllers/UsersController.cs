using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var usersViewModel = users.Select(user => new
        {
            user.Id,
            user.Email,
            Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
        }).ToList();

        ViewBag.Users = usersViewModel;
        ViewBag.Roles = _roleManager.Roles.ToList();

        return View();
    }

    public async Task<IActionResult> Edit(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);
        var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (!string.IsNullOrEmpty(currentRole))
        {
            await _userManager.RemoveFromRoleAsync(user, currentRole);
        }
        if (!string.IsNullOrEmpty(roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        return RedirectToAction(nameof(Index));
    }
}


