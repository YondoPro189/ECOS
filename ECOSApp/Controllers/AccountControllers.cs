using Microsoft.AspNetCore.Mvc;
using ECOSApp.Models;

namespace ECOSApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        // Usuarios válidos del sistema (en producción esto debería estar en base de datos)
        private readonly Dictionary<string, string> _validUsers = new()
        {
            { "admin", "admin123" },
            { "juez1", "juez123" },
            { "evaluador", "eval123" },
            { "coordinador", "coord123" }
        };

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al dashboard
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Dashboard", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                // Validar que el modelo sea válido
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Intento de login con modelo inválido");
                    return View(model);
                }

                // Validar que los campos no estén vacíos
                if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                {
                    ViewBag.Error = "Usuario y contraseña son obligatorios";
                    _logger.LogWarning("Intento de login con campos vacíos");
                    return View(model);
                }

                // Limpiar espacios en blanco
                var username = model.Username.Trim();
                var password = model.Password.Trim();

                // Validar longitud mínima
                if (username.Length < 3)
                {
                    ViewBag.Error = "El usuario debe tener al menos 3 caracteres";
                    _logger.LogWarning($"Usuario muy corto: {username}");
                    return View(model);
                }

                if (password.Length < 6)
                {
                    ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                    _logger.LogWarning($"Contraseña muy corta para usuario: {username}");
                    return View(model);
                }

                // Verificar credenciales
                if (_validUsers.TryGetValue(username, out string? validPassword) && validPassword == password)
                {
                    // Login exitoso
                    HttpContext.Session.SetString("Username", username);
                    HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                    _logger.LogInformation($"Login exitoso: {username} en {DateTime.Now}");
                    
                    TempData["Success"] = $"¡Bienvenido, {username}!";
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    // Credenciales inválidas
                    ViewBag.Error = "Usuario o contraseña incorrectos";
                    _logger.LogWarning($"Intento de login fallido para usuario: {username}");
                    
                    // Limpiar el campo de contraseña por seguridad
                    model.Password = string.Empty;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de login");
                ViewBag.Error = "Error al procesar la solicitud. Por favor, intenta de nuevo.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var username = HttpContext.Session.GetString("Username");
            
            // Limpiar sesión
            HttpContext.Session.Clear();
            
            _logger.LogInformation($"Usuario {username} cerró sesión en {DateTime.Now}");
            
            TempData["Success"] = "Has cerrado sesión exitosamente";
            return RedirectToAction("Login");
        }

        // Acción para verificar si el usuario está autenticado (útil para AJAX)
        [HttpGet]
        public IActionResult CheckAuth()
        {
            var username = HttpContext.Session.GetString("Username");
            var isAuthenticated = !string.IsNullOrEmpty(username);
            
            return Json(new { 
                isAuthenticated = isAuthenticated,
                username = username
            });
        }
    }
}