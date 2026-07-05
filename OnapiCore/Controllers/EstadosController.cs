using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadosController : ControllerBase
    {
        private readonly OnapiCoreContext _context;

        public EstadosController(OnapiCoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estado>>> GetEstados()
        {
            return await _context.Estados.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Estado>> PostEstado(Estado estado)
        {
            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEstados), new { id = estado.EstadoId }, estado);
        }
    }
}