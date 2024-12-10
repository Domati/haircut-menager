// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using HaircutManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HaircutManager.Data;
using reCAPTCHA.AspNetCore;

namespace HaircutManager.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly AppDbContext _context;
        private readonly IRecaptchaService _recaptcha;
        private readonly IConfiguration _configuration;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, AppDbContext context, IRecaptchaService recaptcha, IConfiguration configuration)
        {
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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["RecaptchaSiteKey"] = _configuration["GoogleReCAPTCHA:SiteKey"];
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {

                var recaptchaResult = await _recaptcha.Validate(Request);
                if (!recaptchaResult.success)
                {
                    ModelState.AddModelError(string.Empty, "Please verify that you are not a robot.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

                if (result.Succeeded)
                {
                    await LogAuditAsync(
                        "Login","User",user?.Id ?? "Unknown",string.Empty,$"User {Input.Email} successfully logged in.",user?.Email ?? "Unknown");

                    if (User.Claims.Any(c => c.Type == "NeedPasswordChange" && c.Value == "true"))
                    {
                        _logger.LogInformation("User needs to change password on first login.");
                        return RedirectToPage("./Manage/ChangePassword");
                    }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    var reason = result.IsLockedOut
                        ? "Account locked out"
                        : result.RequiresTwoFactor
                            ? "Requires two-factor authentication"
                            : "Invalid login attempt";

                    await LogAuditAsync(
                        "FailedLogin",
                        "User",user?.Id ?? "Unknown",string.Empty,$"Failed login attempt for user {Input.Email}. Reason: {reason}","System"
                    );

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private async Task LogAuditAsync(string action, string entity, string entityId, string oldValue, string newValue, string changedBy)
        {
            var audit = new Audit
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow
            };

            _context.Audit.Add(audit);
            await _context.SaveChangesAsync();
        }

    }
}
