using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly OnapiCoreContext _context;

        public SolicitudesController(OnapiCoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solicitude>>> GetSolicitudes()
        {
            return await _context.Solicitudes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitude>> GetSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound();
            return solicitud;
        }

        [HttpPost]
        public async Task<ActionResult<Solicitude>> PostSolicitud(Solicitude solicitud)
        {
            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.SolicitudId }, solicitud);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolicitud(int id, Solicitude solicitud)
        {
            if (id != solicitud.SolicitudId) return BadRequest();

            _context.Entry(solicitud).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound();

            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}