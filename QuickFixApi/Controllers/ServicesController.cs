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
    }
}
