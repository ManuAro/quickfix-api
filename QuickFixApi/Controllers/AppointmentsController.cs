using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using QuickFixApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace QuickFixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (userId == null || role == null)
            return Unauthorized();

        IQueryable<Appointment> query = _context.Appointments;

        if (role == "client")
            query = query.Where(a => a.ClientId == userId);
        else if (role == "provider")
            query = query.Where(a => a.ProviderId == userId);
        else
            return Forbid();

        var appointments = await query.ToListAsync();
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Appointment>> GetById(int id)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (userId == null || role == null)
            return Unauthorized();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        bool autorizado = role switch
        {
            "client" => appointment.ClientId == userId,
            "provider" => appointment.ProviderId == userId,
            _ => false
        };

        if (!autorizado)
            return Forbid();

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> Create(Appointment appointment)
    {
        DateTime startTime;
        try
        {
            startTime = DateTime.ParseExact($"{appointment.Date} {appointment.Time}", "yyyy-MM-dd HH:mm", null);
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Formato inválido de Date o Time. Esperado: yyyy-MM-dd y HH:mm" });
        }

        DateTime endTime = appointment.EndTime ?? startTime.AddMinutes(30);

        var solapada = await _context.Appointments.AnyAsync(a =>
            a.ProviderId == appointment.ProviderId &&
            a.AcceptedByProvider == true &&
            DateTime.ParseExact($"{a.Date} {a.Time}", "yyyy-MM-dd HH:mm", null) < endTime &&
            a.EndTime > startTime
        );

        if (solapada)
            return Conflict(new { message = "El proveedor ya tiene una cita aceptada en ese horario." });

        appointment.CreatedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.EndTime ??= endTime;

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, Appointment updated)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (userId == null || role == null)
            return Unauthorized();

        if (id != updated.Id)
            return BadRequest();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        bool autorizado = role switch
        {
            "client" => appointment.ClientId == userId,
            "provider" => appointment.ProviderId == userId,
            _ => false
        };

        if (!autorizado)
            return Forbid();

        appointment.ProviderId = updated.ProviderId;
        appointment.ClientId = updated.ClientId;
        appointment.ProviderName = updated.ProviderName;
        appointment.ClientName = updated.ClientName;
        appointment.ProviderProfession = updated.ProviderProfession;
        appointment.Date = updated.Date;
        appointment.Time = updated.Time;
        appointment.Status = updated.Status;
        appointment.Location = updated.Location;
        appointment.Notes = updated.Notes;
        appointment.Price = updated.Price;
        appointment.ServiceDescription = updated.ServiceDescription;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (userId == null || role == null)
            return Unauthorized();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        bool autorizado = role switch
        {
            "client" => appointment.ClientId == userId,
            "provider" => appointment.ProviderId == userId,
            _ => false
        };

        if (!autorizado)
            return Forbid();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/provider-response")]
    [Authorize(Roles = "provider")]
    public async Task<IActionResult> RespondToAppointment(int id, [FromBody] ProviderResponseDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound(new { message = "Appointment not found." });

        if (appointment.ProviderId != userId)
            return Forbid();

        if (appointment.AcceptedByProvider.HasValue)
            return BadRequest(new { message = "El proveedor ya respondió a esta cita." });

        if (dto.AcceptedByProvider && dto.EndTime == null)
            return BadRequest(new { message = "EndTime es requerido al aceptar la cita." });

        var parsedStart = DateTime.ParseExact($"{appointment.Date} {appointment.Time}", "yyyy-MM-dd HH:mm", null);
        if (dto.AcceptedByProvider && parsedStart < DateTime.UtcNow)
            return BadRequest(new { message = "No se puede aceptar una cita que ya pasó." });

        appointment.AcceptedByProvider = dto.AcceptedByProvider;
        appointment.EndTime = dto.EndTime;
        appointment.UpdatedAt = DateTime.UtcNow;

        if (dto.AcceptedByProvider && dto.EndTime != null)
        {
            var availability = await _context.Availabilities
                .Include(a => a.Slots)
                .FirstOrDefaultAsync(a => a.ProviderId == appointment.ProviderId && a.Date == parsedStart.Date);

            if (availability != null)
            {
                var slotsOcupados = GetTimeSlotsBetween(parsedStart, dto.EndTime.Value);
                foreach (var slot in availability.Slots)
                {
                    if (slotsOcupados.Contains(slot.Time))
                        slot.Booked = true;
                }

                _context.Entry(availability).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Respuesta del proveedor registrada correctamente." });
    }
    [HttpPatch("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var userId = GetUserId();
        var role = GetUserRole();

        if (userId == null || role == null)
            return Unauthorized();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        bool autorizado = role switch
        {
            "client" => appointment.ClientId == userId,
            "provider" => appointment.ProviderId == userId,
            _ => false
        };

        if (!autorizado)
            return Forbid();

        if (appointment.Status == "cancelled")
            return BadRequest("La cita ya fue cancelada.");

        if (appointment.Status == "completed")
            return BadRequest("No se puede cancelar una cita ya completada.");

        appointment.Status = "cancelled";
        appointment.UpdatedAt = DateTime.UtcNow;

        if (appointment.AcceptedByProvider == true && appointment.EndTime != null)
        {
            var startTime = DateTime.ParseExact($"{appointment.Date} {appointment.Time}", "yyyy-MM-dd HH:mm", null);
            var endTime = appointment.EndTime.Value;

            var availability = await _context.Availabilities
                .Include(a => a.Slots)
                .FirstOrDefaultAsync(a =>
                    a.ProviderId == appointment.ProviderId &&
                    a.Date.Date == startTime.Date);

            if (availability != null)
            {
                foreach (var slot in availability.Slots)
                {
                    var slotTime = DateTime.ParseExact($"{appointment.Date} {slot.Time}", "yyyy-MM-dd HH:mm", null);
                    if (slotTime >= startTime && slotTime < endTime)
                        slot.Booked = false;
                }

                _context.Entry(availability).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Cita cancelada y slots liberados." });
    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult SecureCheck()
    {
        return Ok("Token válido. Acceso autorizado.");
    }

    public class ProviderResponseDto
    {
        public bool AcceptedByProvider { get; set; }
        public DateTime? EndTime { get; set; }
    }

    private static List<string> GetTimeSlotsBetween(DateTime start, DateTime end)
    {
        var slots = new List<string>();
        while (start < end)
        {
            slots.Add(start.ToString("HH:mm"));
            start = start.AddMinutes(30);
        }
        return slots;
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : null;
    }

    private string? GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }
}
