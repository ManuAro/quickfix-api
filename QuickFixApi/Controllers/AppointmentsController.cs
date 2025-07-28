using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

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
    // Obtener todas las citas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
    {
        return await _context.Appointments.ToListAsync();
    }

    // ✅ GET: /api/appointments/{id}
    // Obtener una cita por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetById(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return NotFound();

        return appointment;
    }

    // ✅ POST: /api/appointments
    // Crear nueva cita
    [HttpPost]
    public async Task<ActionResult<Appointment>> Create(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    // ✅ PUT: /api/appointments/{id}
    // Actualizar una cita existente
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

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ✅ DELETE: /api/appointments/{id}
    // Eliminar una cita
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
// El proveedor responde a una cita (acepta o rechaza)
[HttpPatch("{id}/provider-response")]
public async Task<IActionResult> RespondToAppointment(int id, [FromBody] ProviderResponseDto dto)
{
    var appointment = await _context.Appointments.FindAsync(id);
    if (appointment == null)
        return NotFound(new { message = "Appointment not found." });

    // Validar si ya respondió
    if (appointment.AcceptedByProvider.HasValue)
        return BadRequest(new { message = "Provider has already responded to this appointment." });

    // Si acepta, debe enviar EndTime
    if (dto.AcceptedByProvider == true && dto.EndTime == null)
        return BadRequest(new { message = "EndTime is required when accepting the appointment." });

    appointment.AcceptedByProvider = dto.AcceptedByProvider;
    appointment.EndTime = dto.EndTime;

    await _context.SaveChangesAsync();
    return Ok(new { message = "Provider response recorded successfully." });
}

// ✅ DTO para la respuesta del proveedor
public class ProviderResponseDto
{
    public bool AcceptedByProvider { get; set; }
    public DateTime? EndTime { get; set; }
}
}
