using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Models;
using ECOSApp.Data;

namespace ECOSApp.Controllers
{
    public class EquiposController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EquiposController> _logger;

        public EquiposController(ApplicationDbContext context, ILogger<EquiposController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Equipos
        public async Task<IActionResult> Index()
        {
            try
            {
                var equipos = await _context.Equipos
                    .OrderByDescending(e => e.FechaRegistro)
                    .ToListAsync();
                
                // Pasar la lista a través de ViewBag en lugar del modelo
                ViewBag.Equipos = equipos;
                
                // Pasar un modelo vacío para el formulario de creación
                return View(new Equipo());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipos");
                TempData["Error"] = "Error al cargar los equipos";
                ViewBag.Equipos = new List<Equipo>();
                return View(new Equipo());
            }
        }

        // POST: Equipos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipo equipo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    equipo.FechaRegistro = DateTime.Now;
                    _context.Add(equipo);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Equipo creado exitosamente";
                    _logger.LogInformation($"Equipo creado: {equipo.Nombre}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear equipo");
                TempData["Error"] = "Error al crear el equipo";
            }
            
            ViewBag.Equipos = await _context.Equipos.ToListAsync();
            return View("Index", equipo);
        }

        // GET: Equipos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            return Json(equipo);
        }

        // POST: Equipos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipo equipo)
        {
            if (id != equipo.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var equipoExistente = await _context.Equipos.FindAsync(id);
                    if (equipoExistente == null)
                    {
                        return NotFound();
                    }

                    equipoExistente.Nombre = equipo.Nombre;
                    equipoExistente.Descripcion = equipo.Descripcion;
                    equipoExistente.FotoUrl = equipo.FotoUrl;

                    _context.Update(equipoExistente);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Equipo actualizado exitosamente";
                    _logger.LogInformation($"Equipo actualizado: {equipo.Nombre}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al actualizar equipo");
                TempData["Error"] = "El equipo fue modificado por otro usuario";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar equipo");
                TempData["Error"] = "Error al actualizar el equipo";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Equipos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var equipo = await _context.Equipos.FindAsync(id);
                if (equipo == null)
                {
                    TempData["Error"] = "Equipo no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar si tiene votaciones asociadas
                var tieneVotaciones = await _context.Votaciones
                    .AnyAsync(v => v.EquipoId == id);

                if (tieneVotaciones)
                {
                    TempData["Error"] = "No se puede eliminar el equipo porque tiene votaciones asociadas";
                    return RedirectToAction(nameof(Index));
                }

                _context.Equipos.Remove(equipo);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Equipo eliminado exitosamente";
                _logger.LogInformation($"Equipo eliminado: {equipo.Nombre}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar equipo");
                TempData["Error"] = "Error al eliminar el equipo";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Equipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipos
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (equipo == null)
            {
                return NotFound();
            }

            return View(equipo);
        }
    }
}