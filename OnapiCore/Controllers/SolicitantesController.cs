using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitantesController : ControllerBase
    {
        private readonly OnapiCoreContext _context;

        public SolicitantesController(OnapiCoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solicitante>>> GetSolicitantes()
        {
            return await _context.Solicitantes.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Solicitante>> PostSolicitante(Solicitante solicitante)
        {
            _context.Solicitantes.Add(solicitante);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSolicitantes), new { id = solicitante.SolicitanteId }, solicitante);
        }
    }
}