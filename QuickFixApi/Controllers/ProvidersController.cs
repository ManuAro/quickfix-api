using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;

namespace QuickFixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProvidersController(AppDbContext context)
    {
        _context = context;
    }

    // 🔍 GET: api/providers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Provider>>> GetProviders()
    {
        return await _context.Providers.ToListAsync();
    }

    // 🔍 GET: api/providers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Provider>> GetProvider(int id)
    {
        var provider = await _context.Providers.FindAsync(id);

        if (provider == null)
            return NotFound();

        return provider;
    }

    // 📝 POST: api/providers
    [HttpPost]
    public async Task<ActionResult<Provider>> Post(Provider provider)
    {
        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProvider), new { id = provider.Id }, provider);
    }

    // 🛠️ PUT: api/providers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Provider provider)
    {
        if (id != provider.Id)
            return BadRequest();

        var existing = await _context.Providers.FindAsync(id);
        if (existing == null)
            return NotFound();

        // Copiar todos los campos
        existing.Name = provider.Name;
        existing.Profession = provider.Profession;
        existing.Rating = provider.Rating;
        existing.Reviews = provider.Reviews;
        existing.Location = provider.Location;
        existing.Price = provider.Price;
        existing.Image = provider.Image;
        existing.Description = provider.Description;
        existing.Services = provider.Services;
        existing.Phone = provider.Phone;
        existing.Email = provider.Email;
        existing.Availability = provider.Availability;
        existing.Certifications = provider.Certifications;
        existing.Coordinates = provider.Coordinates;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ❌ DELETE: api/providers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var provider = await _context.Providers.FindAsync(id);
        if (provider == null)
            return NotFound();

        _context.Providers.Remove(provider);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}


