using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;
using QuickFixApi.Helpers;


namespace QuickFixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ POST: api/reviews
    [HttpPost]
    [Authorize(Roles = "client")]
    public async Task<IActionResult> PostReview([FromBody] Review review)
    {
        var userEmail = UserClaimsHelper.GetEmail(User);

        // Validar que la cita existe y pertenece al cliente logueado
        var appointment = await _context.Appointments.FindAsync(review.AppointmentId);
        if (appointment == null || appointment.ClientEmail != userEmail)
            return Forbid();

        if (appointment.Status != "completed")
            return BadRequest("La cita aún no ha sido completada.");

        // Evitar duplicados
        bool reviewExists = await _context.Reviews.AnyAsync(r => r.AppointmentId == review.AppointmentId);
        if (reviewExists)
            return Conflict("Ya existe una reseña para esta cita.");

        // Guardar la review
        review.ProviderId = appointment.ProviderId;
        review.ClientId = appointment.ClientId;
        review.CreatedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(review);
    }

    // ✅ GET: api/reviews/provider/{id}
    [HttpGet("provider/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Review>>> GetByProvider(int id)
    {
        return await _context.Reviews
            .Where(r => r.ProviderId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
