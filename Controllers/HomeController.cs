using ECOS.Web.Data;
using ECOS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ECOS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly EcosDbContext _context;

        public HomeController(EcosDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEquipos = await _context.Equipos.CountAsync(e => e.Activo);
            ViewBag.TotalJueces = await _context.Jueces.CountAsync(j => j.Activo);
            ViewBag.TotalVotos = await _context.Votaciones.CountAsync();
            ViewBag.JuecesQueVotaron = await _context.Jueces.CountAsync(j => j.YaVoto);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}