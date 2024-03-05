using Microsoft.AspNetCore.Mvc;

namespace HaircutManager.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
