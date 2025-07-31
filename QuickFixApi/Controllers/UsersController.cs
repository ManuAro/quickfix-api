using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickFixApi.Data;
using QuickFixApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace QuickFixApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ GET: api/users
    // Solo admin puede ver todos
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    // ✅ GET: api/users/5
    // Solo el propio usuario o admin
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> GetById(int id)
    {
        if (!IsAdmin() && GetUserId() != id)
            return Forbid();

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        return user;
    }

    // ✅ POST: api/users
    // Registro abierto (puede protegerse si querés)
    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // ✅ PUT: api/users/5
    // Solo el propio usuario o admin
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, User user)
    {
        if (id != user.Id)
            return BadRequest();

        if (!IsAdmin() && GetUserId() != id)
            return Forbid();

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Users.Any(e => e.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // ✅ DELETE: api/users/5
    // Solo el propio usuario o admin
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin() && GetUserId() != id)
            return Forbid();

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 🔧 Helpers
    private int? GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(idClaim, out var id) ? id : null;
    }

    private bool IsAdmin()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value == "admin";
    }
    [HttpPatch("{id}/approve")]
[Authorize(Roles = "admin")]
public async Task<IActionResult> ApproveProvider(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null || user.UserType != "provider")
        return NotFound();

    user.Approved = true;
    await _context.SaveChangesAsync();

    return Ok(new { message = "Proveedor aprobado exitosamente." });
}
}
