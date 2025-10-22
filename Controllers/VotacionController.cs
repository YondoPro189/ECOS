using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECOS.Digital.Models;

namespace ECOS.Digital.Controllers
{
    [Authorize(Roles = "Maestro, Invitado")] // SOLO estos roles pueden votar
    public class VotacionController : Controller
    {

        /* // GET: /Votacion
        public async Task<IActionResult> Index()
        {
            /* Cargar los equipos para mostrarlos en la vista de votación
            var equipos = await _context.Equipos.ToListAsync();
            return View(equipos);
         }   */
        

        // POST: /Votacion/EmitirVoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmitirVoto(int equipoId, int puntuacion)
        {
            // 1. Obtener el ID del usuario actual (el votante)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Validar el modelo (puntuación entre 1 y 5)
            if (puntuacion < 1 || puntuacion > 5)
            {
                TempData["Error"] = "Puntuación no válida.";
                return RedirectToAction(nameof(Index));
            }

            /*// 3. VALIDACIÓN CLAVE: Evitar doble voto por equipo
            var votoExistente = await _context.Votos
                .AnyAsync(v => v.EquipoId == equipoId && v.UserId == userId);
                
            if (votoExistente)
            {
                TempData["Error"] = "Ya has emitido tu voto por este equipo.";
                return RedirectToAction(nameof(Index));
            }*/

            // 4. Crear y guardar el voto
            var nuevoVoto = new Voto 
            { 
                EquipoId = equipoId, 
                UserId = userId, 
                Puntuacion = puntuacion, 
                FechaVoto = DateTime.Now 
            };

            /*_context.Votos.Add(nuevoVoto);
            await _context.SaveChangesAsync();*/
            
            TempData["Success"] = "¡Voto registrado con éxito!";
            return RedirectToAction(nameof(Index));
        }
    }
}