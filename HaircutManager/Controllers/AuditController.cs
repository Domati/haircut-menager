using HaircutManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AuditController : Controller
{
    private readonly AppDbContext _context;

    public AuditController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> AuditLog()
    {
        // Pobranie wszystkich danych z tabeli Audit
        var auditLogs = await _context.Audit
            .OrderByDescending(a => a.ChangedAt) // Sortowanie od najnowszych
            .ToListAsync();

        return View(auditLogs);
    }
}
