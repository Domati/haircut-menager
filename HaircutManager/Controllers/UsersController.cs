using HaircutManager.Data;
using HaircutManager.Migrations;
using HaircutManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();

        var usersViewModel = users.Select(user => new
        {
            user.Id,
            user.Email,
            Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
            IsLockedOut = _userManager.IsLockedOutAsync(user).Result
        }).ToList();

        ViewBag.Users = usersViewModel;
        ViewBag.Roles = _roleManager.Roles.ToList();

        return View();
    }

    public async Task<IActionResult> Edit(string id, string roleName)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (!string.IsNullOrEmpty(currentRole))
        {
            await _userManager.RemoveFromRoleAsync(user, currentRole);
        }
        if (!string.IsNullOrEmpty(roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        // Zapis audytu edycji użytkownika
        await LogAuditAsync(
            "Edit",
            $"User {user.Email}",
            user.Id,
            $"Role: {currentRole}",
            $"Role: {roleName}",
            User.Identity.Name
        );

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { Email = model.Email, UserName = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("NeedPasswordChange", "true"));
                if (!string.IsNullOrEmpty(model.RoleName))
                {
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                }

                // Zapis audytu utworzenia użytkownika
                await LogAuditAsync(
                    "Create",
                    "User",
                    user.Id,
                    null,
                    $"Email: {model.Email}, Role: {model.RoleName}",
                    User.Identity.Name
                );

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ViewBag.Roles = _roleManager.Roles.ToList();
        return View(model);
    }

    public async Task<IActionResult> BanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            // Zapis audytu blokady użytkownika
            await LogAuditAsync(
                "Ban",
                "User",
                user.Id,
                null,
                $"User {user.Email} account locked.",
                User.Identity.Name
            );
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UnbanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);

            // Zapis audytu odblokowania użytkownika
            await LogAuditAsync(
                "Unban",
                "User",
                user.Id,
                null,
               $"User {user.Email} account unlocked.",
                User.Identity.Name
            );
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LogAuditAsync(string action, string entity, string entityId, string? oldValue, string newValue, string changedBy)
    {
        var audit = new Audit
        {
            Action = action,
            Entity = entity,
            EntityId = entityId,
            OldValue = oldValue ?? "",
            NewValue = newValue,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow
        };

        // Zapis do bazy danych
        _context.Audit.Add(audit);
        await _context.SaveChangesAsync();
    }
}




//Dodane zostały - tworzenie, Edycja hasła użytkowników, usuwanie użytkowników przez Admina