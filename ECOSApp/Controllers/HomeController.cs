using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Models;
using ECOSApp.Data;
using System.Diagnostics;

namespace ECOSApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Obtener estadísticas generales
                var totalEquipos = await _context.Equipos.CountAsync();
                var totalJueces = await _context.Jueces.CountAsync();
                var totalVotos = await _context.Votaciones.CountAsync();
                
                // Calcular tasa de participación
                var totalPosiblesVotos = totalEquipos * totalJueces;
                var participacion = totalPosiblesVotos > 0 
                    ? (totalVotos * 100.0 / totalPosiblesVotos) 
                    : 0;

                // Equipo con mayor puntuación
                var equiposMejorVotados = await _context.Votaciones
                    .GroupBy(v => new { v.EquipoId, v.EquipoNombre })
                    .Select(g => new
                    {
                        EquipoId = g.Key.EquipoId,
                        EquipoNombre = g.Key.EquipoNombre,
                        PromedioVotos = g.Average(v => v.Puntuacion),
                        TotalVotos = g.Count()
                    })
                    .OrderByDescending(e => e.PromedioVotos)
                    .Take(3)
                    .ToListAsync();

                ViewBag.TotalEquipos = totalEquipos;
                ViewBag.TotalJueces = totalJueces;
                ViewBag.TotalVotos = totalVotos;
                ViewBag.Participacion = Math.Round(participacion, 1);
                ViewBag.EquiposMejorVotados = equiposMejorVotados;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dashboard");
                TempData["Error"] = "Error al cargar las estadísticas";
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}