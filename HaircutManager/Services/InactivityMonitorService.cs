using HaircutManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HaircutManager.Services
{
    public class InactivityMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InactivityMonitorService> _logger;

        public InactivityMonitorService(IServiceScopeFactory scopeFactory, ILogger<InactivityMonitorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                        var inactiveUsers = await userManager.Users
                            .Where(u => u.InactiveLogoutEnabled 
                                    && u.LastActivity < DateTime.UtcNow.Subtract(u.InactiveTimeUntilLogout))
                            .ToListAsync();

                        foreach (var user in inactiveUsers)
                        {
                            await SignOutUser(scope.ServiceProvider, user);
                            _logger.LogInformation($"User {user.Email} signed out due to inactivity.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during inactivity check.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Run every hour
            }
        }

        private async Task SignOutUser(IServiceProvider serviceProvider, ApplicationUser user)
        {
            var signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            await signInManager.SignOutAsync();
        }
    }

}