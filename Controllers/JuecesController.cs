using ECOS.Shared.Models;
using ECOS.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ECOS.Web.Controllers
{
    public class JuecesController : Controller
    {
        private readonly EcosDbContext _context;
        private readonly IWebHostEnvironment _env;

        public JuecesController(EcosDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Jueces
        public async Task<IActionResult> Index()
        {
            var jueces = await _context.Jueces
                .Where(j => j.Activo)
                .ToListAsync();
            return View(jueces);
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

        // GET: Jueces/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jueces/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Usuario,Nombre,Contrasena")] Juez juez, IFormFile? foto)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el usuario no exista
                if (await _context.Jueces.AnyAsync(j => j.Usuario == juez.Usuario))
                {
                    ModelState.AddModelError("Usuario", "El usuario ya existe");
                    return View(juez);
                }

                if (foto != null && foto.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "jueces");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{foto.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    juez.FotoUrl = $"/images/jueces/{uniqueFileName}";
                }

                juez.FechaRegistro = DateTime.Now;
                juez.Activo = true;
                juez.YaVoto = false;

                _context.Add(juez);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(juez);
        }

        // GET: Jueces/Edit/5
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
            return View(juez);
        }

        // POST: Jueces/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Usuario,Nombre,Contrasena,FotoUrl,FechaRegistro,Activo,YaVoto")] Juez juez, IFormFile? foto)
        {
            if (id != juez.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (foto != null && foto.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "jueces");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}_{foto.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await foto.CopyToAsync(stream);
                        }

                        juez.FotoUrl = $"/images/jueces/{uniqueFileName}";
                    }

                    _context.Update(juez);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JuezExists(juez.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(juez);
        }

        // GET: Jueces/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Jueces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juez = await _context.Jueces.FindAsync(id);
            if (juez != null)
            {
                juez.Activo = false;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JuezExists(int id)
        {
            return _context.Jueces.Any(e => e.Id == id);
        }
    }
}