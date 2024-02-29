using Microsoft.EntityFrameworkCore;

namespace StoreMVC.Repositories
{
    public class HomeRepository :IHomeRepository
    {
        private readonly StoreAuthDbContext _db;

        public HomeRepository(StoreAuthDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string sTerm = "", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();

            //Запрос LINQ: Выполняется запрос к базе данных, объединяя таблицы Products и Categories по CategoryId, фильтруя результаты
            //в соответствии с параметрами sTerm и categoryId. Возвращается список продуктов с выбранными полями.
            IEnumerable<Product> products = await (from product in _db.Products
                                  join category in _db.Categories
                                  on product.CategoryId equals category.Id
                                  where string.IsNullOrEmpty(sTerm) || (product != null && product.ProductName.ToLower().Contains(sTerm))
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

