using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    public class OrderController : Controller
    {
                private readonly IOrderRepository _orderRepos;

        public OrderController(IOrderRepository orderRepos)
        {
            _orderRepos = orderRepos;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepos.GetAllOrders();
            return View(orders);
        }
    }
}
