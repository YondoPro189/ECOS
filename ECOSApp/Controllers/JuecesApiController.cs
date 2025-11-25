using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Data;

namespace ECOSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuecesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JuecesApiController> _logger;

        public JuecesApiController(ApplicationDbContext context, ILogger<JuecesApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/jueces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJueces()
        {
            try
            {
                var jueces = await _context.Jueces
                    .OrderBy(j => j.Nombre)
                    .Select(j => new
                    {
                        id = j.Id,
                        nombre = j.Nombre,
                        especialidad = j.Especialidad,
                        descripcion = j.Descripcion,
                        fotoUrl = j.FotoUrl,
                        fechaRegistro = j.FechaRegistro
                    })
                    .ToListAsync();

                return Ok(jueces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jueces");
                return StatusCode(500, "Error al obtener jueces");
            }
        }

        // GET: api/jueces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetJuez(int id)
        {
            try
            {
                var juez = await _context.Jueces.FindAsync(id);

                if (juez == null)
                {
                    return NotFound("Juez no encontrado");
                }

                var juezDetalle = new
                {
                    id = juez.Id,
                    nombre = juez.Nombre,
                    especialidad = juez.Especialidad,
                    descripcion = juez.Descripcion,
                    fotoUrl = juez.FotoUrl,
                    fechaRegistro = juez.FechaRegistro
                };

                return Ok(juezDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener juez");
                return StatusCode(500, "Error al obtener juez");
            }
        }
    }
}