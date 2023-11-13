namespace StoreMVC
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Category>> Categories();
    }
}