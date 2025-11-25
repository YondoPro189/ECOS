using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Data;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EquiposApiController> _logger;

        public EquiposApiController(ApplicationDbContext context, ILogger<EquiposApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/equipos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEquipos()
        {
            try
            {
                var equipos = await _context.Equipos
                    .OrderByDescending(e => e.FechaRegistro)
                    .ToListAsync();

                // Obtener estadísticas de votación para cada equipo
                var equiposConEstadisticas = equipos.Select(e => new
                {
                    id = e.Id,
                    nombre = e.Nombre,
                    descripcion = e.Descripcion,
                    fotoUrl = e.FotoUrl,
                    fechaRegistro = e.FechaRegistro,
                    promedioVotos = _context.Votaciones
                        .Where(v => v.EquipoId == e.Id)
                        .Any()
                        ? Math.Round(_context.Votaciones
                            .Where(v => v.EquipoId == e.Id)
                            .Average(v => v.Puntuacion), 1)
                        : 0,
                    totalVotos = _context.Votaciones.Count(v => v.EquipoId == e.Id),
                    integrantes = new List<object>
                    {
                        new { nombre = "Integrante 1", rol = "Desarrollador", fotoUrl = (string?)null },
                        new { nombre = "Integrante 2", rol = "Diseñador", fotoUrl = (string?)null },
                        new { nombre = "Integrante 3", rol = "Project Manager", fotoUrl = (string?)null }
                    }
                }).ToList();

                return Ok(equiposConEstadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipos");
                return StatusCode(500, "Error al obtener equipos");
            }
        }

        // GET: api/equipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetEquipo(int id)
        {
            try
            {
                var equipo = await _context.Equipos.FindAsync(id);

                if (equipo == null)
                {
                    return NotFound("Equipo no encontrado");
                }

                var votaciones = await _context.Votaciones
                    .Where(v => v.EquipoId == id)
                    .ToListAsync();

                var equipoDetalle = new
                {
                    id = equipo.Id,
                    nombre = equipo.Nombre,
                    descripcion = equipo.Descripcion,
                    fotoUrl = equipo.FotoUrl,
                    fechaRegistro = equipo.FechaRegistro,
                    promedioVotos = votaciones.Any() 
                        ? Math.Round(votaciones.Average(v => v.Puntuacion), 1) 
                        : 0,
                    totalVotos = votaciones.Count,
                    integrantes = new List<object>
                    {
                        new { nombre = "Integrante 1", rol = "Desarrollador", fotoUrl = (string?)null },
                        new { nombre = "Integrante 2", rol = "Diseñador", fotoUrl = (string?)null },
                        new { nombre = "Integrante 3", rol = "Project Manager", fotoUrl = (string?)null }
                    }
                };

                return Ok(equipoDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipo");
                return StatusCode(500, "Error al obtener equipo");
            }
        }
    }
}