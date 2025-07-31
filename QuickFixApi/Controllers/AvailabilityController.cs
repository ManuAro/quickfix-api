using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

namespace QuickFixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly AppDbContext _context;

    public AvailabilityController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ GET: /api/availability
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Availability>>> GetAll()
    {
        return await _context.Availabilities.ToListAsync();
    }

    // ✅ GET: /api/availability/provider/{providerId}
    [HttpGet("provider/{providerId}")]
    public async Task<ActionResult<IEnumerable<Availability>>> GetByProvider(int providerId)
    {
        var availability = await _context.Availabilities
            .Where(a => a.ProviderId == providerId)
            .ToListAsync();

        if (!availability.Any())
            return NotFound();

        return availability;
    }

    // ✅ POST: /api/availability
    [HttpPost]
    public async Task<ActionResult<Availability>> Create(Availability availability)
    {
        _context.Availabilities.Add(availability);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByProvider), new { providerId = availability.ProviderId }, availability);
    }

    // ✅ PUT: /api/availability/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Availability updated)
    {
        if (id != updated.Id)
            return BadRequest();

        var existing = await _context.Availabilities.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.Date = updated.Date;
        existing.Slots = updated.Slots;
        existing.ProviderId = updated.ProviderId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ✅ DELETE: /api/availability/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var availability = await _context.Availabilities.FindAsync(id);
        if (availability == null)
            return NotFound();

        _context.Availabilities.Remove(availability);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ NUEVO: GET /api/availability/provider/{providerId}/available-slots?date=yyyy-MM-dd
    [HttpGet("provider/{providerId}/available-slots")]
    public async Task<IActionResult> GetAvailableSlots(int providerId, [FromQuery] string date)
    {
        // Validar formato de fecha
        if (!DateTime.TryParse(date, out var parsedDate))
            return BadRequest(new { message = "Formato de fecha inválido. Usá yyyy-MM-dd." });

        // Buscar disponibilidad del proveedor en esa fecha
        var availability = await _context.Availabilities
            .FirstOrDefaultAsync(a => a.ProviderId == providerId && a.Date == parsedDate.Date);

        if (availability == null)
            return NotFound(new { message = "No hay disponibilidad cargada para ese día." });

        // Filtrar slots disponibles (no Booked)
        var libres = availability.Slots
            .Where(s => !s.Booked)
            .Select(s => s.Time)
            .ToList();

        return Ok(new
        {
            providerId,
            date = parsedDate.ToString("yyyy-MM-dd"),
            availableSlots = libres
        });
    }
}
