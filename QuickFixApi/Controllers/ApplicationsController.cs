using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

namespace QuickFixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApplicationsController(AppDbContext context)
    {
        _context = context;
    }

    // 🔍 Obtener todas las aplicaciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
    {
        return await _context.Applications.ToListAsync();
    }

    // 🔍 Obtener una aplicación por ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Application>> GetApplication(int id)
    {
        var application = await _context.Applications.FindAsync(id);

        if (application == null)
            return NotFound();

        return application;
    }

    // 📝 Crear nueva aplicación
    [HttpPost]
    public async Task<ActionResult<Application>> PostApplication(Application application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
    }

    // 🛠️ Actualizar una aplicación existente
    [HttpPut("{id}")]
    public async Task<IActionResult> PutApplication(int id, Application application)
    {
        if (id != application.Id)
            return BadRequest();

        var existing = await _context.Applications.FindAsync(id);
        if (existing == null)
            return NotFound();

        // Campos actualizables (ajustá según tu modelo real)
        existing.Name = application.Name;
        existing.Email = application.Email;
        existing.Phone = application.Phone;
        existing.Profession = application.Profession;
        existing.OtherProfession = application.OtherProfession;
        existing.City = application.City;
        existing.OtherCity = application.OtherCity;
        existing.Experience = application.Experience;
        existing.About = application.About;
        // 👇 Solo incluir si tu modelo tiene HasCert
        // existing.HasCert = application.HasCert;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ❌ Eliminar aplicación
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return NotFound();

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
