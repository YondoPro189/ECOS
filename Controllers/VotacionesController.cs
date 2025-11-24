using ECOS.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECOS.Web.Controllers
{
    public class VotacionesController : Controller
    {
        private readonly EcosDbContext _context;

        public VotacionesController(EcosDbContext context)
        {
            _context = context;
        }

        // GET: Votaciones
        public async Task<IActionResult> Index()
        {
            var votaciones = await _context.Votaciones
                .Include(v => v.Juez)
                .Include(v => v.Equipo)
                .OrderByDescending(v => v.FechaVoto)
                .ToListAsync();

            return View(votaciones);
        }

        // GET: Votaciones/Resultados
        public async Task<IActionResult> Resultados()
        {
            var resultados = await _context.Votaciones
                .Include(v => v.Equipo)
                .GroupBy(v => v.Equipo)
                .Select(g => new ResultadoViewModel
                {
                    Equipo = g.Key!,
                    TotalVotos = g.Count(),
                    Porcentaje = 0
                })
                .OrderByDescending(r => r.TotalVotos)
                .ToListAsync();

            var totalVotos = resultados.Sum(r => r.TotalVotos);
            if (totalVotos > 0)
            {
                foreach (var resultado in resultados)
                {
                    resultado.Porcentaje = (resultado.TotalVotos * 100.0) / totalVotos;
                }
            }

            return View(resultados);
        }

        // POST: Votaciones/Resetear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resetear()
        {
            var votaciones = await _context.Votaciones.ToListAsync();
            _context.Votaciones.RemoveRange(votaciones);

            var jueces = await _context.Jueces.ToListAsync();
            foreach (var juez in jueces)
            {
                juez.YaVoto = false;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

    public class ResultadoViewModel
    {
        public ECOS.Shared.Models.Equipo Equipo { get; set; } = null!;
        public int TotalVotos { get; set; }
        public double Porcentaje { get; set; }
    }
}