﻿using HaircutManager.Migrations;
using HaircutManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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
    //TWORZENIE NOWYCH UŻYTKOWNIKÓW
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Roles = _roleManager.Roles.ToList();
        return View(new CreateUserViewModel());
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
                _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("NeedPasswordChange", "true")).Wait();
                {
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                }
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


    //zmiana hasła użytkowników
    // Metoda GET
    public async Task<IActionResult> ChangePassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(new ChangePasswordViewModel { UserId = user.Id });
    }

    // Metoda POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
        if (!isOldPasswordValid)
        {
            ModelState.AddModelError(string.Empty, "Stare hasło jest niepoprawne.");
            return View(model);
        }

        if (user.PasswordHistory.Any(u => u.PasswordHash == _userManager.PasswordHasher.HashPassword(user, model.NewPassword)))
        {
            ModelState.AddModelError(string.Empty, "Hasło już było użyte.");
            return View(model);
        }
        else {

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                if (user.PasswordHistory == null)
                {
                    user.PasswordHistory = new List<OldPassword>();
                }

                user.PasswordHistory.Add(new OldPassword
                {
                    PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword),
                    ChangedAt = DateTime.Now
                });

                await _userManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }

    //USUWANIE UŻYTKOWNIKów
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }

    //BLOKOWANIE UŻYTKOWNIKÓW
    public async Task<IActionResult> BanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
        return RedirectToAction(nameof(Index));
    }

    //ODBLOKOWYWANIE UŻYTKOWNIKÓW
    public async Task<IActionResult> UnbanUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
        }
        return RedirectToAction(nameof(Index));
    }

}



//Dodane zostały - tworzenie, Edycja hasła użytkowników, usuwanie użytkowników przez Admina