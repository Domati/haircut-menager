using HaircutManager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetAllReservations();

    Task<Reservation> GetReservationById(int id);

    Task CreateReservation(Reservation reservation);

    Task UpdateReservation(Reservation reservation);

    Task DeleteReservation(int id);

    Task<bool> ReservationExists(int id);

    Task<List<SelectListItem>> GetServicesAsSelectListItems();
    Task<List<dynamic>> GetReservationEvents();

}
