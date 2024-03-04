using CReshetka.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreMVC;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("StoreAuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StoreAuthDbContextConnection' not found.");

//Внедрение зависимостей(для докера)
//var dbHost = Environment.GetEnvironmentVariable("DB_HOST");//localhost?
//var dbName = Environment.GetEnvironmentVariable("DB_NAME");
//var dbPassword = Environment.GetEnvironmentVariable("DB_MSSQL_SA_PASSWORD");
//var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;password={dbPassword};Trusted_Connection=false;TrustServerCertificate=true;";

var connectionString = $"Data Source=localhost;Initial Catalog=StoreMVC_Auth_db;User ID=sa;password=yourStrong(!)Password;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
builder.Services.AddDbContext<StoreAuthDbContext>(options => options.UseSqlServer(connectionString));

// Настройки Identity

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<StoreAuthDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddControllersWithViews();

// Регистрируем сервисы
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddRazorPages();

// Конфигурация параметров Identity
builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    });

// Создание и настройка приложения
var app = builder.Build();

// Создали админа единожды и закомментили код
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedDefaultDataAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Найстройка промежуточного ПО
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Конфигурация маршрутизации
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//Запуск приложения
app.Run();
