using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index() 
        {
            return View();          
        }

        
    }
}
