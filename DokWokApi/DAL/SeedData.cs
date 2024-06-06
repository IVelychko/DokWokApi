﻿using DokWokApi.BLL;
using DokWokApi.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.DAL;

public static class SeedData
{
    private static Product[] GetProductsToAdd()
    {
        ProductCategory[] categories = [
            new ProductCategory { Name = "Roll" },
            new ProductCategory { Name = "Pizza" },
            new ProductCategory { Name = "Food set" },
            new ProductCategory { Name = "Noodles" },
            new ProductCategory { Name = "Cold beverage" }
        ];

        Product[] products = [
        new Product { Name = "California", Description = "desc", Price = 234, Category = categories[0] },
        new Product { Name = "Red Dragon", Description = "desc", Price = 197, Category = categories[0] },
        new Product { Name = "Pepperoni", Description = "desc", Price = 254, Category = categories[1] },
        new Product { Name = "Texas", Description = "desc", Price = 265, Category = categories[1] },
        new Product { Name = "Food set 1", Description = "desc", Price = 678, Category = categories[2] },
        new Product { Name = "Food set 2", Description = "desc", Price = 721, Category = categories[2] },
        new Product { Name = "Udon", Description = "desc", Price = 178, Category = categories[3] },
        new Product { Name = "Soba", Description = "desc", Price = 180, Category = categories[3] },
        new Product { Name = "Pepsi", Description = "desc", Price = 45, Category = categories[4] },
        ];

        return products;
    }

    private static IdentityRole[] roles = [
            new IdentityRole { Name = UserRoles.Admin },
            new IdentityRole { Name = UserRoles.Customer }
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
    }
}
