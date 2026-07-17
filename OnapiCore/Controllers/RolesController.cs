using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly OnapiCoreContext _context;

        public RolesController(OnapiCoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();
            return rol;
        }

        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRol), new { id = rol.RolId }, rol);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.RolId) return BadRequest();
            _context.Entry(rol).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();
            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}