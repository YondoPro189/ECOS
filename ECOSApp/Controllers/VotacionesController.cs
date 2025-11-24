using Microsoft.AspNetCore.Mvc;

namespace ECOSApp.Controllers
{
    public class VotacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}