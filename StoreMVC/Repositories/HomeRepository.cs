

using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace StoreMVC.Repositories
{
    public class HomeRepository :IHomeRepository
    {
        private readonly StoreAuthDbContext _db;

        public HomeRepository(StoreAuthDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();

            IEnumerable<Product> products = await (from product in _db.Products
                                  join category in _db.Categories
                                  on product.CategoryId equals category.Id
                                  where string.IsNullOrEmpty(sTerm) || (product != null && product.ProductName.ToLower().StartsWith(sTerm))
                                  select new Product
                                  {
                                      Id = product.Id,
                                      ProductName = product.ProductName,
                                      Image = product.Image,
                                      ManufacturerName = product.ManufacturerName,
                                      Description = product.Description,
                                      Price = product.Price,
                                      CategoryId = product.CategoryId,
                                      CategoryName = category.CategoryName
                                  }
                            ).ToListAsync();

            if (categoryId > 0)
            {
                products = products.Where(a => a.CategoryId == categoryId).ToList();
            }

            return products;
        }
    }
}

