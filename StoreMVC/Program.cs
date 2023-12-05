using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreMVC;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StoreAuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StoreAuthDbContextConnection' not found.");

builder.Services.AddDbContext<StoreAuthDbContext>(options => options.UseSqlServer(connectionString));

//��������� Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<StoreAuthDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
//������������ �������
builder.Services.AddTransient<IHomeRepository, HomeRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddRazorPages();

//������������ ���������� Identity
builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    });

//�������� � ��������� ����������
var app = builder.Build();

//������� ������ �������� � ������������ ���
//using (var scope = app.Services.CreateScope())
//{
//    await DbSeeder.SeedDefaultDataAsync(scope.ServiceProvider);
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//���������� �������������� ��
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

//������������ �������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//������ ����������
app.Run();
