using Microsoft.AspNetCore.Mvc;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    public class JuecesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Juez juez)
        {
            if (ModelState.IsValid)
            {
                // Aquí irá la lógica para guardar en BD
                TempData["Success"] = "Juez creado exitosamente";
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Aquí irá la lógica para eliminar de BD
            TempData["Success"] = "Juez eliminado exitosamente";
            return RedirectToAction("Index");
        }
    }
}