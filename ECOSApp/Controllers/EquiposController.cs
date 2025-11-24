using Microsoft.AspNetCore.Mvc;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    public class EquiposController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                // Aquí irá la lógica para guardar en BD
                TempData["Success"] = "Equipo creado exitosamente";
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Aquí irá la lógica para eliminar de BD
            TempData["Success"] = "Equipo eliminado exitosamente";
            return RedirectToAction("Index");
        }
    }
}