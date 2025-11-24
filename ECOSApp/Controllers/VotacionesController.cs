using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Data;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    public class VotacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VotacionesController> _logger;

        public VotacionesController(ApplicationDbContext context, ILogger<VotacionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Votaciones
        public async Task<IActionResult> Index()
        {
            try
            {
                var votaciones = await _context.Votaciones
                    .OrderByDescending(v => v.FechaVoto)
                    .ToListAsync();

                // Calcular estadísticas
                var equipos = await _context.Equipos.ToListAsync();
                var estadisticas = equipos.Select(e => new
                {
                    Equipo = e,
                    Votaciones = votaciones.Where(v => v.EquipoId == e.Id).ToList(),
                    PromedioVotos = votaciones.Where(v => v.EquipoId == e.Id).Any() 
                        ? votaciones.Where(v => v.EquipoId == e.Id).Average(v => v.Puntuacion) 
                        : 0,
                    TotalVotos = votaciones.Count(v => v.EquipoId == e.Id)
                })
                .OrderByDescending(e => e.PromedioVotos)
                .ToList();

                ViewBag.Estadisticas = estadisticas;
                ViewBag.TotalVotos = votaciones.Count;
                ViewBag.TotalEquipos = equipos.Count;
                ViewBag.TotalJueces = await _context.Jueces.CountAsync();

                return View(votaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener votaciones");
                TempData["Error"] = "Error al cargar las votaciones";
                return View(new List<Votacion>());
            }
        }

        // POST: Votaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int equipoId, int juezId, int puntuacion)
        {
            try
            {
                // Validar que el equipo existe
                var equipo = await _context.Equipos.FindAsync(equipoId);
                if (equipo == null)
                {
                    TempData["Error"] = "Equipo no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Validar que el juez existe
                var juez = await _context.Jueces.FindAsync(juezId);
                if (juez == null)
                {
                    TempData["Error"] = "Juez no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Validar puntuación
                if (puntuacion < 1 || puntuacion > 5)
                {
                    TempData["Error"] = "La puntuación debe estar entre 1 y 5";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar si el juez ya votó por este equipo
                var votoExistente = await _context.Votaciones
                    .FirstOrDefaultAsync(v => v.EquipoId == equipoId && v.JuezId == juezId);

                if (votoExistente != null)
                {
                    TempData["Error"] = $"El juez {juez.Nombre} ya votó por el equipo {equipo.Nombre}";
                    return RedirectToAction(nameof(Index));
                }

                var votacion = new Votacion
                {
                    EquipoId = equipoId,
                    EquipoNombre = equipo.Nombre,
                    JuezId = juezId,
                    JuezNombre = juez.Nombre,
                    Puntuacion = puntuacion,
                    FechaVoto = DateTime.Now
                };

                _context.Add(votacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Voto registrado: {juez.Nombre} votó {puntuacion} estrellas para {equipo.Nombre}";
                _logger.LogInformation($"Voto registrado: Juez {juez.Nombre} - Equipo {equipo.Nombre} - Puntuación {puntuacion}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear votación");
                TempData["Error"] = "Error al registrar el voto";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Votaciones/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var votacion = await _context.Votaciones.FindAsync(id);
                if (votacion == null)
                {
                    TempData["Error"] = "Votación no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                _context.Votaciones.Remove(votacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Votación eliminada exitosamente";
                _logger.LogInformation($"Votación eliminada: {votacion.JuezNombre} - {votacion.EquipoNombre}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar votación");
                TempData["Error"] = "Error al eliminar la votación";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Votaciones/GetEstadisticas
        [HttpGet]
        public async Task<IActionResult> GetEstadisticas()
        {
            try
            {
                var votaciones = await _context.Votaciones.ToListAsync();
                var equipos = await _context.Equipos.ToListAsync();

                var estadisticas = equipos.Select(e => new
                {
                    equipoId = e.Id,
                    equipoNombre = e.Nombre,
                    promedioVotos = votaciones.Where(v => v.EquipoId == e.Id).Any()
                        ? votaciones.Where(v => v.EquipoId == e.Id).Average(v => v.Puntuacion)
                        : 0,
                    totalVotos = votaciones.Count(v => v.EquipoId == e.Id),
                    votaciones = votaciones.Where(v => v.EquipoId == e.Id).Select(v => new
                    {
                        juezNombre = v.JuezNombre,
                        puntuacion = v.Puntuacion,
                        fecha = v.FechaVoto
                    }).ToList()
                })
                .OrderByDescending(e => e.promedioVotos)
                .ToList();

                return Json(new { success = true, data = estadisticas });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return Json(new { success = false, message = "Error al obtener estadísticas" });
            }
        }
    }
}