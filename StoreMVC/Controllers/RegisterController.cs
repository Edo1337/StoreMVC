using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Registration()
        {
            return View();
        }       
        public IActionResult Check()
        {
            return View();
        }
        public IActionResult Registration_End()
        {
            return View();
        }
    }
}
