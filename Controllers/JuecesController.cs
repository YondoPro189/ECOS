using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOS.Digital.Data;
using ECOS.Digital.Models;

namespace ECOS.Digital.Controllers
{
    [Authorize]
    public class JuecesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JuecesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lista de Jueces (Maestros e Invitados)
        [Authorize(Roles = "Admin, Maestro, Invitado")]
        public async Task<IActionResult> Index()
        {
            var jueces = await _context.Users
                .Where(u => u.UserType == "Maestro" || u.UserType == "Invitado")
                .OrderBy(u => u.UserType)
                .ThenBy(u => u.UserName)
                .ToListAsync();

            return View(jueces);
        }

        // GET: Visualización de todos los votos
        [Authorize(Roles = "Admin, Maestro, Invitado")]
        public async Task<IActionResult> Votos()
        {
            var votos = await _context.Votos
                .Include(v => v.Equipo)
                .Include(v => v.UsuarioVotante)
                .OrderByDescending(v => v.FechaVoto)
                .ToListAsync();

            return View(votos);
        }
    }
}