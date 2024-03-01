namespace StoreMVC.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddProductAsync(int productId, int qty);
        Task<int> RemoveProductAsync(int productId);
        Task<ShoppingCart> GetUserCart();
        Task<int> GetCartProductCountAsync(string userId = "");
        Task<bool> DoCheckout();
        Task<ShoppingCart> GetCartAsync(string userId);
        Task<bool> ClearCart();

    }
}