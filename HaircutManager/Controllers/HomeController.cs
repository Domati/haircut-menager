using HaircutManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HaircutManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceRepository _serviceRepository;

        public HomeController(ILogger<HomeController> logger, IServiceRepository serviceRepository)
        {
            _logger = logger;
            _serviceRepository = serviceRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var services = await _serviceRepository.ListAsync();
            return View(services);
        }

        public IActionResult Contact()
        {
            return View();
        }

    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
