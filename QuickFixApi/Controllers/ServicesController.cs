using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QuickFixApi.Models;
using QuickFixApi.Data;

namespace QuickFixApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Público: Obtener todos los servicios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _context.Services.ToListAsync();
            return Ok(services);
        }

        // ✅ Solo admin: Crear un nuevo servicio
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = service.Id }, service);
        }

        // ✅ Solo admin: Editar un servicio
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, Service service)
        {
            var existing = await _context.Services.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = service.Name;
            existing.Description = service.Description;
            existing.WorkerId = service.WorkerId;
            existing.Category = service.Category;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ✅ Solo admin: Eliminar un servicio
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
