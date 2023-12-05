using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace StoreMVC.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly StoreAuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserOrderRepository(StoreAuthDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Пользователь не вошел в систему");
            var orders = await _db.Orders
                                    .Include(x=>x.OrderStatus)
                                    .Include(x=>x.OrderDetail)
                                    .ThenInclude(x=>x.Product)
                                    .ThenInclude(x=>x.Category)
                                    .Where(a => a.UserId == userId)
                                    .ToListAsync();
            return orders;
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
