using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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
    // Público: devuelve toda la disponibilidad (puede usarse para debugging o admin panel)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Availability>>> GetAll()
    {
        return await _context.Availabilities.ToListAsync();
    }

    // ✅ GET: /api/availability/provider/{providerId}
    // Público: muestra toda la disponibilidad de un proveedor
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

    // ⚠️ Solo Admin (o reservado para pruebas): crear disponibilidad manualmente
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Availability>> Create(Availability availability)
    {
        _context.Availabilities.Add(availability);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByProvider), new { providerId = availability.ProviderId }, availability);
    }

    // ⚠️ Solo Admin: actualizar disponibilidad (normalmente no se usa en producción)
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
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

    // ⚠️ Solo Admin: eliminar disponibilidad (no se usa desde frontend)
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var availability = await _context.Availabilities.FindAsync(id);
        if (availability == null)
            return NotFound();

        _context.Availabilities.Remove(availability);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ GET /api/availability/provider/{providerId}/available-slots?date=yyyy-MM-dd
    // Público: muestra solo los horarios libres de un proveedor en una fecha
    [HttpGet("provider/{providerId}/available-slots")]
    public async Task<IActionResult> GetAvailableSlots(int providerId, [FromQuery] string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
            return BadRequest(new { message = "Formato de fecha inválido. Usá yyyy-MM-dd." });

        var availability = await _context.Availabilities
            .FirstOrDefaultAsync(a => a.ProviderId == providerId && a.Date == parsedDate.Date);

        if (availability == null)
            return NotFound(new { message = "No hay disponibilidad cargada para ese día." });

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
