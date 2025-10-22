using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOS.Digital.Models;
using ECOS.Digital.Data;
using System.Security.Claims;

namespace ECOS.Digital.Controllers
{
    [Authorize]
    public class EquiposController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquiposController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, Maestro, Invitado")]
        public async Task<IActionResult> ListaEquipos()
        {
            var equipos = await _context.Equipos
                .Include(e => e.AlumnoLider)
                .Include(e => e.VotosRecibidos)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            return View(equipos);
        }

        [Authorize(Roles = "Alumno")]
        public async Task<IActionResult> Manage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var equipoExistente = await _context.Equipos
                .FirstOrDefaultAsync(e => e.AlumnoLiderId == userId);

            if (equipoExistente != null)
            {
                return View(equipoExistente);
            }
            
            var nuevoEquipo = new Equipo
            {
                AlumnoLiderId = userId
            };
            
            return View(nuevoEquipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Alumno")]
        public async Task<IActionResult> Manage(Equipo equipo)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            equipo.AlumnoLiderId = userId;

            if (!ModelState.IsValid)
            {
                return View(equipo);
            }

            try
            {
                if (equipo.Id == 0)
                {
                    _context.Equipos.Add(equipo);
                    TempData["Success"] = "¡Proyecto registrado exitosamente!";
                }
                else
                {
                    _context.Equipos.Update(equipo);
                    TempData["Success"] = "¡Proyecto actualizado exitosamente!";
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar el proyecto: " + ex.Message;
                return View(equipo);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Registrar(Equipo equipo)
        {
            if (!ModelState.IsValid)
            {
                return View(equipo);
            }

            try
            {
                _context.Equipos.Add(equipo);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = $"Equipo '{equipo.Nombre}' registrado exitosamente.";
                return RedirectToAction(nameof(Registrar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el equipo: " + ex.Message;
                return View(equipo);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .Include(e => e.AlumnoLider)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var equipo = await _context.Equipos.FindAsync(id);
                if (equipo != null)
                {
                    _context.Equipos.Remove(equipo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Equipo eliminado exitosamente.";
                }
                return RedirectToAction(nameof(ListaEquipos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el equipo: " + ex.Message;
                return RedirectToAction(nameof(ListaEquipos));
            }
        }
    }
}