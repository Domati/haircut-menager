using HaircutManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HaircutManager.Areas.Identity.Pages.Account
{
    public class LoginWithOTPModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string Email { get; set; }

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginWithOTPModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "OTP Answer")]
            public string OtpAnswer { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            Email = email;
            //var user = await _userManager.FindByEmailAsync(Email);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email, string returnUrl = null)
        {
            Email = email;
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByEmailAsync(email);
                var user = await _userManager.Users
                                        .Include(u => u.OneTimePasswords)
                                        .FirstOrDefaultAsync(u => u.Email == email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    var otp = user.OneTimePasswords.Select(o => o).FirstOrDefault(o => !o.IsUsed);
                    var otpClaim = claims.FirstOrDefault(c => c.Type == "OtpClaim");
                    if (otp.Answer == Input.OtpAnswer)
                    {
                        //updating the OTP to avoid reusing it
                        otp.IsUsed = true;
                        await _userManager.UpdateAsync(user);
                        //remove the otp claim
                        await _userManager.RemoveClaimAsync(user, otpClaim);
                        //sign in the user
                        //await _signInManager.SignInAsync(user, isPersistent: false);

                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        //redirect to "set new password" page
                        return RedirectToPage("./ResetPassword", new { email = Email, code = token});
                    }
                    ModelState.AddModelError(string.Empty, "Invalid OTP.");
                    return Page();
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            return Page();
        }
    }

}
