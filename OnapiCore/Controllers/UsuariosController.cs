using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly OnapiCoreContext _context;

        public UsuariosController(OnapiCoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Select(u => new { u.UsuarioId, u.NombreUsuario, u.RolId, u.FechaCreacion })
                .ToListAsync();
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto datos)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == datos.NombreUsuario);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(datos.Clave, usuario.ClaveHash))
            {
                return Unauthorized(new { mensaje = "Usuario o clave incorrectos" });
            }

            return Ok(new { usuario.UsuarioId, usuario.NombreUsuario, usuario.RolId });
        }
        [HttpPost]
        public async Task<ActionResult> PostUsuario(RegistroUsuarioDto datos)
        {
            var usuario = new Usuario
            {
                NombreUsuario = datos.NombreUsuario,
                ClaveHash = BCrypt.Net.BCrypt.HashPassword(datos.Clave),
                RolId = datos.RolId
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { usuario.UsuarioId, usuario.NombreUsuario });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, RegistroUsuarioDto datos)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.NombreUsuario = datos.NombreUsuario;
            usuario.RolId = datos.RolId;

            if (!string.IsNullOrEmpty(datos.Clave))
            {
                usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(datos.Clave);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    public class LoginDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Clave { get; set; } = null!;
    }
    public class RegistroUsuarioDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Clave { get; set; } = null!;
        public int RolId { get; set; }
    }

}