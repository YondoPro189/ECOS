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
                // Por ahora, cualquier usuario/contraseña funcionará
                if (!string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password))
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}