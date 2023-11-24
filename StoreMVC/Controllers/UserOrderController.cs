using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepos;

        public UserOrderController(IUserOrderRepository userOrderRepos)
        {
            _userOrderRepos = userOrderRepos;
        }
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRepos.UserOrders();
            return View(orders);
        }
    }
}
