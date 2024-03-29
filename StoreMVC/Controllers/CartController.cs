﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StoreMVC.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddProduct(int productId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddProductAsync(productId, qty);
            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            var cartCount = await _cartRepo.RemoveProductAsync(productId);

            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            
            return View(cart);
        }
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartProduct = await _cartRepo.GetCartProductCountAsync();

            return Ok(cartProduct);
        }
        public async Task<IActionResult> Checkout()
        {
            bool isCheckedOut = await _cartRepo.DoCheckout();
            if (!isCheckedOut)
                throw new Exception("Проблема на стороне сервера");
            return RedirectToAction("UserOrders", "UserOrder");
        }
        
        public async Task<IActionResult> ClearCart()
        {
            bool isCheckedOut = await _cartRepo.ClearCart();
            if (!isCheckedOut)
                throw new Exception("Проблема на стороне сервера");
            return RedirectToAction("Index", "Home");
        }

    }
}
