using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECOSApp.Models;
using ECOSApp.Data;

namespace ECOSApp.Controllers
{
    public class JuecesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JuecesController> _logger;

        public JuecesController(ApplicationDbContext context, ILogger<JuecesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Jueces
        public async Task<IActionResult> Index()
        {
            try
            {
                var jueces = await _context.Jueces
                    .OrderByDescending(j => j.FechaRegistro)
                    .ToListAsync();
                
                // Pasar la lista a través de ViewBag en lugar del modelo
                ViewBag.Jueces = jueces;
                
                // Pasar un modelo vacío para el formulario de creación
                return View(new Juez());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jueces");
                TempData["Error"] = "Error al cargar los jueces";
                ViewBag.Jueces = new List<Juez>();
                return View(new Juez());
            }
        }

        // POST: Jueces/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Juez juez)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    juez.FechaRegistro = DateTime.Now;
                    _context.Add(juez);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Juez creado exitosamente";
                    _logger.LogInformation($"Juez creado: {juez.Nombre}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear juez");
                TempData["Error"] = "Error al crear el juez";
            }
            
            ViewBag.Jueces = await _context.Jueces.ToListAsync();
            return View("Index", juez);
        }

        // GET: Jueces/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juez = await _context.Jueces.FindAsync(id);
            if (juez == null)
            {
                return NotFound();
            }

            return Json(juez);
        }

        // POST: Jueces/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Juez juez)
        {
            if (id != juez.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var juezExistente = await _context.Jueces.FindAsync(id);
                    if (juezExistente == null)
                    {
                        return NotFound();
                    }

                    juezExistente.Nombre = juez.Nombre;
                    juezExistente.Especialidad = juez.Especialidad;
                    juezExistente.Descripcion = juez.Descripcion;
                    juezExistente.FotoUrl = juez.FotoUrl;

                    _context.Update(juezExistente);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Juez actualizado exitosamente";
                    _logger.LogInformation($"Juez actualizado: {juez.Nombre}");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al actualizar juez");
                TempData["Error"] = "El juez fue modificado por otro usuario";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar juez");
                TempData["Error"] = "Error al actualizar el juez";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Jueces/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var juez = await _context.Jueces.FindAsync(id);
                if (juez == null)
                {
                    TempData["Error"] = "Juez no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar si tiene votaciones asociadas
                var tieneVotaciones = await _context.Votaciones
                    .AnyAsync(v => v.JuezId == id);

                if (tieneVotaciones)
                {
                    TempData["Error"] = "No se puede eliminar el juez porque tiene votaciones asociadas";
                    return RedirectToAction(nameof(Index));
                }

                _context.Jueces.Remove(juez);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Juez eliminado exitosamente";
                _logger.LogInformation($"Juez eliminado: {juez.Nombre}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar juez");
                TempData["Error"] = "Error al eliminar el juez";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Jueces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juez = await _context.Jueces
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (juez == null)
            {
                return NotFound();
            }

            return View(juez);
        }
    }
}