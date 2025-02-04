﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using reCAPTCHA.AspNetCore;

namespace HaircutManager.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly AppDbContext _context;
        private readonly IRecaptchaService _recaptcha;
        private readonly IConfiguration _configuration;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            AppDbContext context, IRecaptchaService recaptcha, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _recaptcha = recaptcha;
            _configuration = configuration;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ViewData["RecaptchaSiteKey"] = _configuration["ApiKeys:GoogleReCAPTCHA:SiteKey"];


        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {


                var recaptchaResult = await _recaptcha.Validate(Request);
                if (!recaptchaResult.success)
                {
                    ModelState.AddModelError(string.Empty, "Please verify that you are not a robot.");
                    return Page();
                }
            }

            var user = await _userManager.Users
                                         .Include(u => u.PasswordHistory)
                                         .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.PasswordHistory == null)
            {
                user.PasswordHistory = new List<OldPassword>();
            }

            if (user.PasswordHistory.Any(p => _userManager.PasswordHasher
            .VerifyHashedPassword(user, p.PasswordHash, Input.NewPassword) == PasswordVerificationResult.Success))
            {
                ModelState.AddModelError(string.Empty, "Password already used.");
                StatusMessage = "Password already used.";
                return Page();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                // Zapisujemy nowe hasło do historii
                var newOldPassword = new OldPassword
                {
                    id = Guid.NewGuid(),
                    PasswordHash = _userManager.PasswordHasher.HashPassword(user, Input.NewPassword),
                    ChangedAt = DateTime.Now,
                    UserId = user.Id,
                    User = user
                };

                user.PasswordHistory.Add(newOldPassword);

                await _userManager.UpdateAsync(user);

                // Logowanie audytu zmiany hasła
                await LogAuditAsync(
                    "ChangePassword",
                    "User",
                    user.Id,
                    null, // Brak starego hasła w audycie
                    $"Password changed for user '{user.Email}'", // Nowe hasło z informacją o użytkowniku
                    User.Identity.Name
                );
            }
            else
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            // Remove claim
            await _userManager.RemoveClaimAsync(user, new Claim("NeedPasswordChange", "true"));

            return RedirectToPage();
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
}
