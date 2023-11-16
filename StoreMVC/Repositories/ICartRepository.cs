namespace StoreMVC.Repositories
{
    public interface ICartRepository
    {
        Task<bool> AddProductAsync(int productId, int qty);
        Task<bool> RemoveProductAsync(int productId);
        Task<IEnumerable<ShoppingCart>> GetUserCart();
    }
}