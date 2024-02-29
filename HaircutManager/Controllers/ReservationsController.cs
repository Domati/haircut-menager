using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HaircutManager.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }
        //Wyświetlanie listy rezerwacji 
        public async Task<IActionResult> List()
        {
            return View(await _context.Reservations.ToListAsync());
        }


        // Wyświetlanie szczegółów rezerwacji
        public async Task<IActionResult> Details(int? id)
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

        // Tworzenie nowej rezerwacji - GET
        public IActionResult Create()
        {
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName");
            // Tutaj możesz załadować listę usług, jeśli jest potrzebna w formularzu
            return View();
        }

        // Tworzenie nowej rezerwacji - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,ReservationName,ReservationDate,ClientName,ClientEmail,ClientPhoneNumber,ServiceId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Tutaj załaduj listę usług, jeśli jest potrzebna w formularzu
            return View();
        }



        // Edycja rezerwacji - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            // Tutaj załaduj listę usług, jeśli jest potrzebna w formularzu
            return View(reservation);
        }

        // Edycja rezerwacji - POST
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
                return RedirectToAction(nameof(Index));
            }
            // Tutaj załaduj listę usług, jeśli jest potrzebna
            return View(reservation);
        }

        // Pomocnicza metoda do sprawdzania, czy rezerwacja istnieje
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
