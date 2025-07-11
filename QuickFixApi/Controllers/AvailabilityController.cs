using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

namespace QuickFixApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AvailabilityController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/availability
        // Obtener todas las disponibilidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Availability>>> GetAll()
        {
            return await _context.Availabilities.ToListAsync();
        }

        // ✅ GET: api/availability/provider/5
        // Obtener disponibilidad de un proveedor específico
        [HttpGet("provider/{providerId}")]
        public async Task<ActionResult<IEnumerable<Availability>>> GetByProvider(int providerId)
        {
            var result = await _context.Availabilities
                .Where(a => a.ProviderId == providerId)
                .ToListAsync();

            return result;
        }

        // ✅ POST: api/availability
        // Crear nueva disponibilidad
        [HttpPost]
        public async Task<ActionResult<Availability>> Create(Availability availability)
        {
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = availability.Id }, availability);
        }

        // ✅ PUT: api/availability/5
        // Actualizar disponibilidad existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Availability updated)
        {
            if (id != updated.Id)
                return BadRequest();

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Availabilities.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // ✅ DELETE: api/availability/5
        // Eliminar disponibilidad
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
    }
}
