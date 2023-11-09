using Microsoft.AspNetCore.Identity;
using StoreMVC.Constants;

namespace StoreMVC.Areas.Identity.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultDataAsync(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<ApplicationUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            //Добавляем роли в базу данных
            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            //Создание пользователя-админа
            var admin = new ApplicationUser
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            };

            var userInDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDb is null)
            {
                await userMgr.CreateAsync(admin, "Perfect_passw0rd"); //Аккуратно с паролем, очень важно указывать пароль, подходящий по требованиям (кто бы мог подумать, 1 час с этим ебався..)
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }

    }
}
