using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HaircutManager.Data;
using HaircutManager.Models;

namespace HaircutManager.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly AppDbContext _context;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, AppDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Pobieranie informacji o zalogowanym użytkowniku
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name;

            // Zapis do audytu przed wylogowaniem
            await LogAuditAsync(
                "Logout","User", userId,string.Empty,$"User {userName} logged out.",userName
            );

            // Wykonanie wylogowania
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }

        private async Task LogAuditAsync(string action, string entity, string entityId, string oldValue, string newValue, string changedBy)
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
