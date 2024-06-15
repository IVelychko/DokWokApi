using DokWokApi.BLL;
using DokWokApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL;

public static class SeedData
{
    private static Product[] GetProductsToAdd()
    {
        ProductCategory[] categories = [
            new() { Name = "Roll" },
            new() { Name = "Pizza" },
            new() { Name = "Food set" },
            new() { Name = "Noodles" },
            new() { Name = "Cold beverage" }
        ];

        Product[] products = [
        new() { Name = "California", Description = "desc", Price = 234, Weight = 200, MeasurementUnit = "г", Category = categories[0] },
        new() { Name = "Red Dragon", Description = "desc", Price = 197, Weight = 250, MeasurementUnit = "г", Category = categories[0] },
        new() { Name = "Pepperoni", Description = "desc", Price = 254, Weight = 500, MeasurementUnit = "г", Category = categories[1] },
        new() { Name = "Texas", Description = "desc", Price = 265, Weight = 520, MeasurementUnit = "г", Category = categories[1] },
        new() { Name = "Food set 1", Description = "desc", Price = 678, Weight = 1200, MeasurementUnit = "г", Category = categories[2] },
        new() { Name = "Food set 2", Description = "desc", Price = 721, Weight = 1600, MeasurementUnit = "г", Category = categories[2] },
        new() { Name = "Udon", Description = "desc", Price = 178, Weight = 320, MeasurementUnit = "г", Category = categories[3] },
        new() { Name = "Soba", Description = "desc", Price = 180, Weight = 340, MeasurementUnit = "г", Category = categories[3] },
        new() { Name = "Pepsi", Description = "desc", Price = 45, Weight = 500, MeasurementUnit = "г", Category = categories[4] },
        ];

        return products;
    }

    private static IdentityRole[] roles = [
            new() { Name = UserRoles.Admin },
            new() { Name = UserRoles.Customer }
        ];

    private static Shop[] shops = [
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
        context.Database.Migrate();

        if (!roleManager.Roles.Any())
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

        if (!context.ProductCategories.Any() && !context.Products.Any())
        {
            context.Products.AddRange(GetProductsToAdd());
            context.SaveChanges();
        }

        if (!context.Shops.Any())
        {
            context.Shops.AddRange(shops);
            context.SaveChanges();
        }
    }
}
