using HaircutManager.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HaircutManager.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ActivityTrackerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivityTrackerMiddleware(RequestDelegate next, UserManager<ApplicationUser> userManager)
        {
            _next = next;
            _userManager = userManager;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            var userId = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.LastActivity = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ActivityTrackerMiddlewareExtensions
    {
        public static IApplicationBuilder UseActivityTrackerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActivityTrackerMiddleware>();
        }
    }
}
