using Microsoft.AspNetCore.Mvc;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validar credenciales específicas
                if (model.Username == "admin" && model.Password == "adminECOS")
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos. Use: admin / adminECOS";
                    return View(model);
                }
            }
            
            ViewBag.Error = "Por favor complete todos los campos";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}