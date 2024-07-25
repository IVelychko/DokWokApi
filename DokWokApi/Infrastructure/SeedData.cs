using DokWokApi.BLL.Infrastructure;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.Infrastructure;

public static class SeedData
{
    private static Product[] GetProductsToAdd()
    {
        ProductCategory[] categories = [
            new() { Name = "Роли" },
            new() { Name = "Піца" },
            new() { Name = "Сет їжі" },
            new() { Name = "Локшина" },
            new() { Name = "Прохолодні напої" }
        ];

        Product[] products = [
            new() { Name = "Зелений дракон", Description = "Вугор, ікра масаго, сир, огірок, авокадо", 
                Price = 269, Weight = 255, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Філадельфія", Description = "Лосось, сир Філадельфія, огірок", Price = 289, Weight = 235, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Золотий дракон", 
                Description = "Вугор, сир Філадельфія, огірок, ікра Масаго, Унаги с-с", Price = 269, Weight = 240, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Удон з куркою та овочами", 
                Description = "Пшенична локшина, куряче філе, цибуля, болгарський перець, пекінська капуста, морква, печериці, соєвий соус, соус 'солодкий чилі', часник, кунжут", 
                Price = 119, Weight = 140, MeasurementUnit = "г", Category = categories[3] },
            new() { Name = "Харусаме з куркою та овочами", 
                Description = "Рисова локшина, куряче філе, цибуля ріпчаста, болгарський перець, пекінська капуста, печериці, морква, соєвий соус, часник, імбир, соус 'солодкий чилі' кунжут", 
                Price = 129, Weight = 140, MeasurementUnit = "г", Category = categories[3] },
            new() { Name = "Соба з куркою та овочами", 
                Description = "Гречана локшина, куряче філе, цибуля, болгарський перець, пекінська капуста, морква, печериці, соєвий соус, соус 'солодкий чилі', часник, імбри, кунжут", 
                Price = 129, Weight = 140, MeasurementUnit = "г", Category = categories[3] },
            new() { Name = "Пепсі 0.5 л", Description = "Напій безалкогольний сильногазований 'Pepsi'", Price = 19, Weight = 500, MeasurementUnit = "г", Category = categories[4] },
            new() { Name = "7up 0.5 л", Description = "Напій безалкогольний сильногазований '7up'", Price = 19, Weight = 500, MeasurementUnit = "г", Category = categories[4] },
            new() { Name = "Мірінда 0.5 л", Description = "Напій безалкогольний сильногазований 'Mirinda'", Price = 19, Weight = 500, MeasurementUnit = "г", Category = categories[4] },
            new() { Name = "Сет три дракона", Description = "Рол Червоний дракон, Рол Зелений дракон, Рол Золотий дракон", Price = 699, Weight = 400, MeasurementUnit = "г", Category = categories[2] },
            new() { Name = "Каліфорнія в кунжуті", Description = "Лосось, сир Філадельфія, огірок, кунжут", Price = 189, Weight = 230, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Червоний дракон", Description = "Лосось, вугор, сир Філадельфія, огірок, майонез", Price = 269, Weight = 280, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Капрезо", Description = "Соус фірмовий вершково-соєвий, сир Моцарела, помідор, печериці, маслини", Price = 99, Weight = 430, MeasurementUnit = "г", Category = categories[1] },
            new() { Name = "Карбонара", Description = "Вершки ,сир Моцарела, шинка, бекон, сир Пармезан, яєчний жовток", Price = 139, Weight = 420, MeasurementUnit = "г", Category = categories[1] },
            new() { Name = "Пепероні", Description = "Соус фірмовий томатний, сир Моцарела, пепероні, помідор", Price = 149, Weight = 450, MeasurementUnit = "г", Category = categories[1] },
        ];

        return products;
    }

    private readonly static IdentityRole[] roles = [
            new() { Name = UserRoles.Admin },
            new() { Name = UserRoles.Customer }
        ];

    private readonly static Shop[] shops = [
            new() { Street = "Олександра Поля", Building = "36", OpeningTime = "10:00", ClosingTime = "22:00" },
            new() { Street = "Незалежності", Building = "42", OpeningTime = "09:00", ClosingTime = "21:00" },
            new() { Street = "Дмитра Яворницького", Building = "12", OpeningTime = "10:00", ClosingTime = "21:00" },
            new() { Street = "Робоча", Building = "54", OpeningTime = "10:00", ClosingTime = "22:00" },
        ];

    public static async Task SeedDatabaseAsync(IApplicationBuilder app)
    {
        var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
        var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await context.Database.MigrateAsync();

        if (!await roleManager.Roles.AnyAsync())
        {
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }

        var admins = await userManager.GetUsersInRoleAsync(UserRoles.Admin);
        if (admins.Count < 1)
        {
            await userManager.CreateAsync(new ApplicationUser
            {
                FirstName = "Ihor",
                UserName = "Admin1",
                Email = "admin@example.com",
                PhoneNumber = "1234567890"
            }, "AdminPassword1");
            var admin = await userManager.FindByNameAsync("Admin1");
            await userManager.AddToRoleAsync(admin!, UserRoles.Admin);
        }

        if (!await context.ProductCategories.AnyAsync() && !await context.Products.AnyAsync())
        {
            context.Products.AddRange(GetProductsToAdd());
            await context.SaveChangesAsync();
        }

        if (!await context.Shops.AnyAsync())
        {
            context.Shops.AddRange(shops);
            await context.SaveChangesAsync();
        }
    }
}
