using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HaircutManager.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        //Wyświetlanie listy rezerwacji 
        public async Task<IActionResult> List()
        {
            return View(await _context.Reservations.Include(r => r.Service).ToListAsync());
        }


        // Wyświetlanie szczegółów rezerwacji
        [Authorize(Roles = "Admin,Fryzjer")]
        public async Task<IActionResult> Details(int? id)
        {
            TempData["PreviousUrl"] = HttpContext.Request.GetTypedHeaders().Referer?.ToString();
            var previousUrl = TempData["PreviousUrl"] as string;

            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // Tworzenie nowej rezerwacji - GET
        public IActionResult Create()
        {
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }

        // Tworzenie nowej rezerwacji - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,ReservationDate,ClientName,ClientEmail,ClientPhoneNumber,ServiceId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var service = await _context.Services.FindAsync(reservation.ServiceId);
                if (service != null)
                {
                    // Obliczanie przewidywanej godziny zakończenia
                    reservation.EstimatedEndTime = reservation.ReservationDate.AddMinutes(service.AvgTimeOfService);
                }

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }

            return View(reservation);
        }


        [Authorize(Roles = "Admin,Fryzjer")]
        // Edycja rezerwacji - GET
        public async Task<IActionResult> Edit(int? id)
        {

            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName");
            if (id == null)
            {
                return NotFound();
            }



            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(reservation.ServiceId);
            if (service != null)
            {
                // Obliczanie przewidywanej godziny zakończenia
                reservation.EstimatedEndTime = reservation.ReservationDate.AddMinutes(service.AvgTimeOfService);
            }

            return View(reservation);
        }

        // Edycja rezerwacji - POST
        [Authorize(Roles = "Admin,Fryzjer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,ReservationName,ReservationDate,ClientName,ClientEmail,ClientPhoneNumber,ServiceId")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var service = await _context.Services.FindAsync(reservation.ServiceId);
                    if (service != null)
                    {
                        reservation.EstimatedEndTime = reservation.ReservationDate.AddMinutes(service.AvgTimeOfService);
                    }

                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(List));
            }
            return View(reservation);
        }

        // Pomocnicza metoda do sprawdzania, czy rezerwacja istnieje
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }

        // Wyświetlanie potwierdzenia usunięcia rezerwacji - GET
        [Authorize(Roles = "Admin,Fryzjer")]
        public async Task<IActionResult> Delete(int? id)
        { 
            
               
            if (id == null)
            {
                return NotFound();
            }
                var reservation = await _context.Reservations
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
           
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // Usuwanie rezerwacji - POST
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Fryzjer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(List));
        }

        //Przesylanie rezerwacji do kalendarza

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var reservations = await _context.Reservations.Include(r => r.Service).Select(r => new
                {
                    id = r.ReservationId,
                    title = r.ClientName,
                    service = r.Service.ServiceName,
                    start = r.ReservationDate, 
                    end = r.EstimatedEndTime,
                })
                .ToListAsync();

            return Json(reservations);
        }

    }
}
