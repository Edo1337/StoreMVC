using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    public class CatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Food_For_Cat()
        {
            return View();
        }
    }
}
