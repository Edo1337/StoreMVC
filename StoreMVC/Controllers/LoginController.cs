using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Sign_in()
        {
            return View();
        }
    }
}
