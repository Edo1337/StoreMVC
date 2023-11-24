using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Areas.Identity.Data;

namespace StoreMVC.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly StoreAuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(StoreAuthDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddProductAsync(int productId, int qty)
        {
            string userId = GetUserId();
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Пользователь не вошел в систему");
                var cart = await GetCartAsync(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    await _db.ShoppingCarts.AddAsync(cart);//у него не асинхр
                }
                _db.SaveChanges();
                //cart detail section
                var cartProduct = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);
                if (cartProduct is not null)
                    cartProduct.Quantity += qty;
                else
                {
                    var product = _db.Products.Find(productId);
                    cartProduct = new CartDetail
                    {
                        ProductId = productId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = product.Price
                    };
                    await _db.CartDetails.AddAsync(cartProduct);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {

            }

            var cartProductCount = await GetCartProductCountAsync(userId);
            return cartProductCount;
        }

        public async Task<int> RemoveProductAsync(int productId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Пользователь не вошел в систему");
                var cart = await GetCartAsync(userId);
                if (cart is null)
                    throw new Exception("Корзина недействительна");
                //cart detail section
                var cartProduct = _db.CartDetails
                                     .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);

                if (cartProduct is null)
                    throw new Exception("В корзине нет товаров");
                else if (cartProduct.Quantity == 1)
                    _db.CartDetails.Remove(cartProduct);
                else
                    cartProduct.Quantity = cartProduct.Quantity - 1;

                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            var cartProductCount = await GetCartProductCountAsync(userId);
            return cartProductCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                throw new Exception("Неправильный ID пользователя");
            }
            var shoppingCart = await _db.ShoppingCarts
                .Include(a => a.CartDetails)
                .ThenInclude(a => a.Product)
                .ThenInclude(a => a.Category)
                .Where(a => a.UserId == userId).FirstOrDefaultAsync();

            return shoppingCart;

        }

        public async Task<ShoppingCart> GetCartAsync(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> GetCartProductCountAsync(string userId = "") //общее количество элементов в карточке
        {
            if (!string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              select new { cartDetail.Id }
                        ).ToListAsync();

            return data.Count;
        }

        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // Логика
                // передаем данные из cartDetail to order and orderDetail далее удаляем данные из cartDetail
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Пользователь не вошел в систему");

                var cart = await GetCartAsync(userId);
                if (cart is null)
                    throw new Exception("Корзина не действительная");

                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();

                if (cartDetail.Count == 0)
                    throw new Exception("Корзина пустая");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = 1, //pending или же ожидание
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();

                // удаление cartdetails
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            return userId;
        }

    }
}
