using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HaircutManager.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepository;

        public ServicesController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // Wyświetlanie listy usług
        public async Task<IActionResult> List()
        {
            return View(await _serviceRepository.ListAsync());
        }

        [Authorize(Roles = "Admin,Fryzjer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Fryzjer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceName,Description,Price,AvgTimeOfService")] Service service)
        {
            if (ModelState.IsValid)
            {
                await _serviceRepository.AddAsync(service);
                return RedirectToAction(nameof(List));
            }
            return View(service);
        }

        [Authorize(Roles = "Admin,Fryzjer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.FindByIdAsync(id.Value);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Fryzjer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,ServiceName,Description,Price,AvgTimeOfService")] Service service)
        {
            if (id != service.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!_serviceRepository.Exists(service.ServiceId))
                {
                    return NotFound();
                }
                await _serviceRepository.UpdateAsync(service);
                return RedirectToAction(nameof(List));
            }
            return View(service);
        }

        [Authorize(Roles = "Admin,Fryzjer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.FindByIdAsync(id.Value);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Fryzjer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _serviceRepository.FindByIdAsync(id);
            if (service != null)
            {
                await _serviceRepository.DeleteAsync(service);
            }
            return RedirectToAction(nameof(List));
        }
    }
}
