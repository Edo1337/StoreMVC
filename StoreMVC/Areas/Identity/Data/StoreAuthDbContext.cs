using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Areas.Identity.Data;
using StoreMVC.Models;

namespace StoreMVC.Data;

public class StoreAuthDbContext : IdentityDbContext<ApplicationUser>
{
    public StoreAuthDbContext(DbContextOptions<StoreAuthDbContext> options)
        : base(options)
    {
    }

    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ShoppingCart> ShoppingCarts { get; set; }
    DbSet<CartDetail> CartDetails { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderDetail> OrderDetails { get; set; }
    DbSet<OrderStatus> OrderStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
