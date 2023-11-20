using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Areas.Identity.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Security.Claims;

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
                    cartProduct = new CartDetail
                    {
                        ProductId = productId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,

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

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            return userId;
        }

    }
}
