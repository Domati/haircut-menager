using HaircutManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HaircutManager.Areas.Identity.Pages.Account.Manage
{
    public class SetNewPasswordModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public string Email { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SetNewPasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public class InputModel
        {
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            Email = email;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            Email = email;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user != null)
                {
                    var result = await _userManager.AddPasswordAsync(user, Input.NewPassword);
                    if (result.Succeeded)
                    {
                        // Remove the "NeedPasswordChange" claim if it exists
                        if (User.Claims.Any(c => c.Type == "NeedPasswordChange" && c.Value == "true"))
                        {
                            _userManager.RemoveClaimAsync(user, User.Claims.FirstOrDefault(c => c.Type == "NeedPasswordChange"));
                        }
                        // Sign in the user
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToPage("./Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                ModelState.AddModelError(string.Empty, "Unable to find user.");
            }

            return Page();
        }
    }

}
