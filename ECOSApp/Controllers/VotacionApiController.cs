using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Data;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotacionesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VotacionesApiController> _logger;

        public VotacionesApiController(ApplicationDbContext context, ILogger<VotacionesApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/votaciones/votar
        [HttpPost("votar")]
        public async Task<IActionResult> Votar([FromBody] VotacionRequest request)
        {
            try
            {
                // Validar datos
                if (request.EquipoId <= 0 || request.JuezId <= 0 || request.Puntuacion < 1 || request.Puntuacion > 5)
                {
                    return BadRequest("Datos de votación inválidos");
                }

                // Verificar que el equipo existe
                var equipo = await _context.Equipos.FindAsync(request.EquipoId);
                if (equipo == null)
                {
                    return NotFound("Equipo no encontrado");
                }

                // Verificar que el juez existe
                var juez = await _context.Jueces.FindAsync(request.JuezId);
                if (juez == null)
                {
                    return NotFound("Juez no encontrado");
                }

                // Verificar si ya votó
                var votoExistente = await _context.Votaciones
                    .FirstOrDefaultAsync(v => v.EquipoId == request.EquipoId && v.JuezId == request.JuezId);

                if (votoExistente != null)
                {
                    return Conflict($"El juez {juez.Nombre} ya votó por el equipo {equipo.Nombre}");
                }

                // Crear votación
                var votacion = new Votacion
                {
                    EquipoId = request.EquipoId,
                    EquipoNombre = equipo.Nombre,
                    JuezId = request.JuezId,
                    JuezNombre = juez.Nombre,
                    Puntuacion = request.Puntuacion,
                    FechaVoto = DateTime.Now
                };

                _context.Votaciones.Add(votacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Voto registrado: {juez.Nombre} votó {request.Puntuacion} estrellas para {equipo.Nombre}");

                return Ok(new
                {
                    success = true,
                    message = "Voto registrado exitosamente",
                    data = new
                    {
                        equipoNombre = equipo.Nombre,
                        juezNombre = juez.Nombre,
                        puntuacion = request.Puntuacion,
                        fecha = votacion.FechaVoto
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar votación");
                return StatusCode(500, $"Error al registrar votación: {ex.Message}");
            }
        }

        // GET: api/votaciones/verificar?equipoId=1&juezId=2
        [HttpGet("verificar")]
        public async Task<IActionResult> VerificarVoto(int equipoId, int juezId)
        {
            try
            {
                var yaVoto = await _context.Votaciones
                    .AnyAsync(v => v.EquipoId == equipoId && v.JuezId == juezId);

                return Ok(new
                {
                    success = true,
                    data = yaVoto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar voto");
                return StatusCode(500, "Error al verificar voto");
            }
        }
    }

    // Modelos de Request
    public class VotacionRequest
    {
        public int EquipoId { get; set; }
        public int JuezId { get; set; }
        public int Puntuacion { get; set; }
    }
}