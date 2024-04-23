using HaircutManager.Data;
using HaircutManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Service>> ListAsync()
    {
        return await _context.Services.ToListAsync();
    }

    public async Task<Service> FindByIdAsync(int id)
    {
        return await _context.Services.FindAsync(id);
    }

    public async Task AddAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Service service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Service service)
    {
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
    }

    public bool Exists(int id)
    {
        return _context.Services.Any(e => e.ServiceId == id);
    }
}
