using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace StoreMVC.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreAuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderRepository(StoreAuthDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var orders = await _db.Orders
                                    .Include(x => x.OrderStatus)
                                    .Include(x => x.OrderDetail)
                                    .ThenInclude(x => x.Product)
                                    .ThenInclude(x => x.Category)
                                    .ToListAsync();
            return orders;
        }

    }
}
