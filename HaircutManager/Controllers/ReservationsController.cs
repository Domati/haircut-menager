using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol.Core.Types;

namespace HaircutManager.Controllers
{
    public class ReservationsController : Controller
    {


        private readonly IReservationRepository _reservationRepository;
        private readonly IServiceRepository _serviceRepository;

        public ReservationsController(IReservationRepository reservationRepository, IServiceRepository serviceRepository)
        {
            _reservationRepository = reservationRepository;
            _serviceRepository = serviceRepository;
        }



        [Authorize]
        //Wyświetlanie listy rezerwacji 
        public async Task<IActionResult> List()
        {
            return View( await _reservationRepository.GetAllReservations());
        }


        // Wyświetlanie szczegółów rezerwacji
        [Authorize(Roles = "Admin,Fryzjer")]
        public async Task<IActionResult> Details(int id)
        {
            TempData["PreviousUrl"] = HttpContext.Request.GetTypedHeaders().Referer?.ToString();
            var previousUrl = TempData["PreviousUrl"] as string;

            if (id == null)
            {
                return NotFound();
            }

            var reservation =  await _reservationRepository.GetReservationById(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // Tworzenie nowej rezerwacji - GET
        public async Task<IActionResult> CreateAsync()
        {

            var serviceItems = await _reservationRepository.GetServicesAsSelectListItems();


            ViewBag.ServiceId = new SelectList(serviceItems, "Value", "Text");

            return View();
        }

        // Tworzenie nowej rezerwacji - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,ReservationDate,ClientName,ClientEmail,ClientPhoneNumber,ServiceId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var service = await _serviceRepository.FindByIdAsync(reservation.ServiceId);
                if (service != null)
                {
                    // Obliczanie przewidywanej godziny zakończenia
                    reservation.EstimatedEndTime = reservation.ReservationDate.AddMinutes(service.AvgTimeOfService);
                }

                await _reservationRepository.CreateReservation(reservation);
                return RedirectToAction(nameof(List));
            }

            return View(reservation);
        }


        [Authorize(Roles = "Admin,Fryzjer")]
        // Edycja rezerwacji - GET
        public async Task<IActionResult> Edit(int id)
        {

            var serviceItems = await _reservationRepository.GetServicesAsSelectListItems();

            ViewBag.ServiceId = new SelectList(serviceItems, "Value", "Text");
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _reservationRepository.GetReservationById(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.FindByIdAsync(reservation.ServiceId);

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

                    var service = await _serviceRepository.FindByIdAsync(reservation.ServiceId);
                    if (service != null)
                    {
                        reservation.EstimatedEndTime = reservation.ReservationDate.AddMinutes(service.AvgTimeOfService);
                    }

                    await _reservationRepository.UpdateReservation(reservation);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _reservationRepository.ReservationExists(reservation.ReservationId))
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



        // Wyświetlanie potwierdzenia usunięcia rezerwacji - GET
        [Authorize(Roles = "Admin,Fryzjer")]
        public async Task<IActionResult> Delete(int id)
        {


            if (id == null)
            {
                return NotFound();
            }
            var reservation = await _reservationRepository.GetReservationById(id);

            return View(reservation);
        }

        // Usuwanie rezerwacji - POST
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Fryzjer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reservationRepository.DeleteReservation(id);

            return RedirectToAction(nameof(List));
        }

        //Przesylanie rezerwacji do kalendarza

        //[HttpGet]
        //public async Task<IActionResult> GetEvents()
        //{
        //    var reservations = await _context.Reservations.Include(r => r.Service).Select(r => new
        //        {
        //            id = r.ReservationId,
        //            title = r.ClientName,
        //            service = r.Service.ServiceName,
        //            start = r.ReservationDate, 
        //            end = r.EstimatedEndTime,
        //        })
        //        .ToListAsync();

        //    return Json(reservations);
        //}

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var reservations = await _reservationRepository.GetReservationEvents();

            return Json(reservations);
        }

    }
}
