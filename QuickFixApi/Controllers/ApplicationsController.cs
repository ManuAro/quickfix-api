using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
    {
        if (IsAdmin())
        {
            return await _context.Applications.ToListAsync();
        }

        var email = GetUserEmail();
        if (email == null)
            return Unauthorized();

        var apps = await _context.Applications
            .Where(a => a.Email == email)
            .ToListAsync();

        return Ok(apps);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Application>> GetApplication(int id)
    {
        var email = GetUserEmail();
        if (email == null)
            return Unauthorized();

        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return NotFound();

        if (!IsAdmin() && application.Email != email)
            return Forbid();

        return application;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Application>> PostApplication(Application application)
    {
        var email = GetUserEmail();
        if (email == null)
            return Unauthorized();

        application.Email = email;
        application.CreatedAt = DateTime.UtcNow;

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutApplication(int id, Application application)
    {
        if (id != application.Id)
            return BadRequest();

        var email = GetUserEmail();
        if (email == null)
            return Unauthorized();

        var existing = await _context.Applications.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (!IsAdmin() && existing.Email != email)
            return Forbid();

        // Actualizar campos
        existing.Name = application.Name;
        existing.Phone = application.Phone;
        existing.Profession = application.Profession;
        existing.OtherProfession = application.OtherProfession;
        existing.City = application.City;
        existing.OtherCity = application.OtherCity;
        existing.Experience = application.Experience;
        existing.About = application.About;
        existing.HasCertifications = application.HasCertifications;
        existing.HasTools = application.HasTools;
        existing.AcceptTerms = application.AcceptTerms;
        existing.Status = application.Status;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var email = GetUserEmail();
        if (email == null)
            return Unauthorized();

        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return NotFound();

        if (!IsAdmin() && application.Email != email)
            return Forbid();

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 🔧 Helpers

    private string? GetUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value;
    }

    private bool IsAdmin()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value == "admin";
    }
}

