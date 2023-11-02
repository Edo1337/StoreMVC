using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreMVC.Areas.Identity.Data;
using StoreMVC.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StoreAuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StoreAuthDbContextConnection' not found.");

builder.Services.AddDbContext<StoreAuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<StoreAuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
