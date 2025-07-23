using Microsoft.AspNetCore.Mvc;
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

        // ✅ Obtener todos los servicios
        [HttpGet]
        public IActionResult GetAll()
        {
            var services = _context.Services.ToList();
            return Ok(services);
        }

        // ✅ Agregar un nuevo servicio
        [HttpPost]
        public IActionResult Create(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetAll), new { id = service.Id }, service);
        }

        // ✅ Editar un servicio existente
        [HttpPut("{id}")]
        public IActionResult Update(int id, Service service)
        {
            var existing = _context.Services.Find(id);
            if (existing == null)
                return NotFound();

            existing.Name = service.Name;
            existing.Description = service.Description;
            existing.WorkerId = service.WorkerId;
            existing.Category = service.Category;

            _context.SaveChanges();
            return Ok(existing);
        }

        // ✅ Eliminar un servicio
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

