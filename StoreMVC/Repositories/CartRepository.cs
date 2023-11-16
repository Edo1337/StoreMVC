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
        private readonly UserManager<ApplicationUser> _userManager;//
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(StoreAuthDbContext db, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> AddProductAsync(int productId, int qty)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }
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
                {
                    cartProduct.Quantity += qty;
                }
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveProductAsync(int productId)
        {
            try
            {
                string userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }
                var cart = await GetCartAsync(userId);
                if (cart is null)
                {
                    return false;
                }
                _db.SaveChanges();
                //cart detail section
                var cartProduct = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);

                if (cartProduct is null)
                {
                    return false;
                }
                else if (cartProduct.Quantity == 1)
                {
                    _db.CartDetails.Remove(cartProduct);
                }
                else
                {
                    cartProduct.Quantity = cartProduct.Quantity - 1;
                }

                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<ShoppingCart>> GetUserCart()
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
                .Where(a => a.UserId == userId).ToListAsync();

            return shoppingCart;

        }

        private async Task<ShoppingCart> GetCartAsync(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
