using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DokWokApi.Helpers;

public static class DatabaseHelper
{
    public static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await SeedUserRolesToDatabaseAsync(context);
        await SeedUsersToDatabaseAsync(context, passwordHasher);
        await SeedProductsAndCategoriesToDatabaseAsync(context);
        await SeedShopsToDatabaseAsync(context);
    }

    private static async Task SeedUserRolesToDatabaseAsync(StoreDbContext context)
    {
        if (!await context.UserRoles.AnyAsync())
        {
            var userRoles = SeedData.Roles.Select(r => new UserRole { Name = r });
            await context.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedUsersToDatabaseAsync(StoreDbContext context, IPasswordHasher passwordHasher)
    {
        var adminExists = await context.Users
            .AnyAsync(u => u.UserRole!.Name == UserRoles.Admin);
        if (!adminExists)
        {
            var adminRole = await context.UserRoles
                .FirstOrDefaultAsync(r => r.Name == UserRoles.Admin);
            var hashedPassword = passwordHasher.Hash("Password1");
            await context.AddAsync(new User
            {
                FirstName = "Ihor",
                UserName = "Admin1",
                Email = "admin@example.com",
                PhoneNumber = "1234567890",
                UserRoleId = adminRole!.Id,
                PasswordHash = hashedPassword
            });
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedProductsAndCategoriesToDatabaseAsync(StoreDbContext context)
    {
        if (!await context.ProductCategories.AnyAsync() && 
            !await context.Products.AnyAsync())
        {
            context.Products.AddRange(SeedData.GetProductsToAdd());
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedShopsToDatabaseAsync(StoreDbContext context)
    {
        if (!await context.Shops.AnyAsync())
        {
            context.Shops.AddRange(SeedData.Shops);
            await context.SaveChangesAsync();
        }
    }
}