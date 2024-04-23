using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetAllReservations()
    {
        return await _context.Reservations.Include(r => r.Service).ToListAsync();
    }

    public async Task<Reservation> GetReservationById(int id)
    {
        return await _context.Reservations
            .Include(r => r.Service)
            .FirstOrDefaultAsync(r => r.ReservationId == id);
    }

    public async Task CreateReservation(Reservation reservation)
    {
        _context.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReservation(Reservation reservation)
    {
        _context.Update(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReservation(int id)
    {
        var reservation = await GetReservationById(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ReservationExists(int id)
    {
        return await _context.Reservations.AnyAsync(e => e.ReservationId == id);
    }

    public async Task<List<SelectListItem>> GetServicesAsSelectListItems()
    {
        return await _context.Services.Select(s => new SelectListItem
        {
            Value = s.ServiceId.ToString(),
            Text = s.ServiceName
        }).ToListAsync();
    }

    public async Task<List<dynamic>> GetReservationEvents()
    {
        var reservation = await _context.Reservations.Include(r => r.Service).Select(r => new
        {
            id = r.ReservationId,
            title = r.ClientName,
            service = r.Service.ServiceName,
            start = r.ReservationDate,
            end = r.EstimatedEndTime
        }).ToListAsync();

        return reservation.Cast<dynamic>().ToList();
    }

  
}
