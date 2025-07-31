using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using QuickFixApi.Models.Requests;

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

    // ✅ GET: /api/appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
    {
        return await _context.Appointments.ToListAsync();
    }

    // ✅ GET: /api/appointments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetById(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        return appointment;
    }

    // ✅ POST: /api/appointments (con validación de superposición)
    [HttpPost]
    public async Task<ActionResult<Appointment>> Create(Appointment appointment)
    {
        // Paso 1: Combinar Date y Time
        DateTime startTime;
        try
        {
            startTime = DateTime.ParseExact($"{appointment.Date} {appointment.Time}", "yyyy-MM-dd HH:mm", null);
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Formato inválido de Date o Time. Esperado: yyyy-MM-dd y HH:mm" });
        }

        // Paso 2: Calcular endTime (30 min por defecto si no se proporciona)
        DateTime endTime = appointment.EndTime ?? startTime.AddMinutes(30);

        // Paso 3: Verificar si hay citas superpuestas del mismo proveedor que ya fueron aceptadas
        var solapada = await _context.Appointments.AnyAsync(a =>
            a.ProviderId == appointment.ProviderId &&
            a.AcceptedByProvider == true &&
            DateTime.ParseExact($"{a.Date} {a.Time}", "yyyy-MM-dd HH:mm", null) < endTime &&
            a.EndTime > startTime
        );

        if (solapada)
            return Conflict(new { message = "El proveedor ya tiene una cita aceptada en ese horario." });

        // Paso 4: Guardar cita
        appointment.CreatedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;
        appointment.EndTime ??= endTime;

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    // ✅ PUT: /api/appointments/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Appointment updated)
    {
        if (id != updated.Id)
            return BadRequest();

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        // Actualizar campos
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

    // ✅ DELETE: /api/appointments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ PATCH: /api/appointments/{id}/provider-response
    [HttpPatch("{id}/provider-response")]
    public async Task<IActionResult> RespondToAppointment(int id, [FromBody] ProviderResponseDto dto)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound(new { message = "Appointment not found." });

        if (appointment.AcceptedByProvider.HasValue)
            return BadRequest(new { message = "Provider has already responded to this appointment." });

        if (dto.AcceptedByProvider && dto.EndTime == null)
            return BadRequest(new { message = "EndTime is required when accepting the appointment." });

        appointment.AcceptedByProvider = dto.AcceptedByProvider;
        appointment.EndTime = dto.EndTime;
        appointment.UpdatedAt = DateTime.UtcNow;

        // 🚨 Bloquear slots en Availability si se acepta la cita
        if (appointment.AcceptedByProvider == true && appointment.EndTime != null)
        {
            try
            {
                var parsedStart = DateTime.ParseExact($"{appointment.Date} {appointment.Time}", "yyyy-MM-dd HH:mm", null);
                var appointmentDate = parsedStart.Date;

                var availability = await _context.Availabilities
                    .FirstOrDefaultAsync(a => a.ProviderId == appointment.ProviderId && a.Date == appointmentDate);

                if (availability != null)
                {
                    var slotsOcupados = GetTimeSlotsBetween(parsedStart, appointment.EndTime.Value);

                    foreach (var slot in availability.Slots)
                    {
                        if (slotsOcupados.Contains(slot.Time))
                            slot.Booked = true;
                    }

                    _context.Entry(availability).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Error al convertir Date y Time. Formato esperado: yyyy-MM-dd y HH:mm" });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Provider response recorded successfully." });
    }

    // DTO
    public class ProviderResponseDto
    {
        public bool AcceptedByProvider { get; set; }
        public DateTime? EndTime { get; set; }
    }

    // Utilidad: generar lista de bloques de 30 minutos
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
 [HttpPatch("{id}/cancel")]
public async Task<IActionResult> CancelAppointment(int id)
{
    var appointment = await _context.Appointments.FindAsync(id);
    if (appointment == null)
        return NotFound();

    if (appointment.Status == "cancelled")
        return BadRequest("La cita ya fue cancelada.");

    appointment.Status = "cancelled";
    appointment.UpdatedAt = DateTime.UtcNow;

    if (appointment.AcceptedByProvider == true && appointment.EndTime != null)
    {
        // Parsear los tiempos
        var startTime = DateTime.Parse($"{appointment.Date} {appointment.Time}");
        var endTime = appointment.EndTime.Value;

        // Buscar disponibilidad del proveedor para ese día
        var availability = await _context.Availabilities
            .Include(a => a.Slots)
            .FirstOrDefaultAsync(a =>
                a.ProviderId == appointment.ProviderId &&
                a.Date.Date == startTime.Date);

        if (availability != null)
        {
            foreach (var slot in availability.Slots)
            {
                var slotTime = DateTime.Parse($"{appointment.Date} {slot.Time}");
                if (slotTime >= startTime && slotTime < endTime)
                {
                    slot.Booked = false;
                }
            }
        }
    }

    await _context.SaveChangesAsync();
    return Ok(new { message = "Cita cancelada y slots liberados." });
}
}
