namespace StoreMVC.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreAuthDbContext _db;
        public ProductRepository(StoreAuthDbContext db)
        {
            _db = db;
        }
        public IEnumerable<Product> Products => throw new NotImplementedException();

        public void SaveProduct(Product product)
        {
            if (product.Id == 0)
                _db.Products.Add(product);
            else
            {
                Product dbEntry = _db.Products.Find(product.Id);
                if (dbEntry != null)
                {
                    dbEntry.ProductName = product.ProductName;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                }
            }
            _db.SaveChanges();
        }
    }
}
