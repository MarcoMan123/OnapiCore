using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TiposDeRegistroController : ControllerBase
    {
        private readonly OnapiCoreContext _context;

        public TiposDeRegistroController(OnapiCoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TiposDeRegistro>>> GetTipos()
        {
            return await _context.TiposDeRegistros.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<TiposDeRegistro>> PostTipo(TiposDeRegistro tipo)
        {
            _context.TiposDeRegistros.Add(tipo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTipos), new { id = tipo.TipoId }, tipo);
        }
    }
}