using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Shared;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DokWokApiTests;

public static class TestHelper
{
    public static string GetRandomString(int minLength, int maxLength)
    {
        Random res = new();
        string str = "abcdefghijklmnopqrstuvwxyz";
        StringBuilder randomString = new();
        int length = res.Next(minLength, maxLength + 1);

        for (int i = 0; i < length; i++)
        {
            int x = res.Next(str.Length);
            randomString.Append(str[x]);
        }

        return randomString.ToString();
    }

    public static DbContextOptions<StoreDbContext> GetDbContextOptions()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase("TestDB")
                .Options;

        using (var context = new StoreDbContext(options))
        {
            SeedDatabase(context);
        }

        return options;
    }

    //private static void SeedDatabase(StoreDbContext context)
    //{
    //    if (!context.ProductCategories.Any() && !context.Products.Any())
    //    {
    //        context.Products.AddRange(GetTestProducts());
    //        context.SaveChanges();
    //    }
    //}

    private readonly static string[] roles = [
            UserRoles.Admin,
            UserRoles.Customer
    ];

    private static ProductCategory[] GetSeedCategories()
    {
        ProductCategory[] categories = [
            new() { Name = "Роли" },
            new() { Name = "Піца" },
            new() { Name = "Сет їжі" },
            new() { Name = "Локшина" },
            new() { Name = "Прохолодні напої" }
        ];

        return categories;
    }

    private static Product[] GetSeedProducts()
    {
        var categories = GetSeedCategories();

        Product[] products = [
            new() { Name = "Зелений дракон", Description = "Вугор, ікра масаго, сир, огірок, авокадо",
                Price = 269, Weight = 255, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Філадельфія", Description = "Лосось, сир Філадельфія, огірок", Price = 289, Weight = 235, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Золотий дракон",
                Description = "Вугор, сир Філадельфія, огірок, ікра Масаго, Унаги с-с", Price = 269, Weight = 240, MeasurementUnit = "г", Category = categories[0] },
                new() { Name = "Каліфорнія в кунжуті", Description = "Лосось, сир Філадельфія, огірок, кунжут", Price = 189, Weight = 230, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Червоний дракон", Description = "Лосось, вугор, сир Філадельфія, огірок, майонез", Price = 269, Weight = 280, MeasurementUnit = "г", Category = categories[0] },
            new() { Name = "Капрезо", Description = "Соус фірмовий вершково-соєвий, сир Моцарела, помідор, печериці, маслини", Price = 99, Weight = 430, MeasurementUnit = "г", Category = categories[1] },
            new() { Name = "Карбонара", Description = "Вершки ,сир Моцарела, шинка, бекон, сир Пармезан, яєчний жовток", Price = 139, Weight = 420, MeasurementUnit = "г", Category = categories[1] },
            new() { Name = "Пепероні", Description = "Соус фірмовий томатний, сир Моцарела, пепероні, помідор", Price = 149, Weight = 450, MeasurementUnit = "г", Category = categories[1] },
            new() { Name = "Сет три дракона", Description = "Рол Червоний дракон, Рол Зелений дракон, Рол Золотий дракон", Price = 699, Weight = 400, MeasurementUnit = "г", Category = categories[2] },
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
        ];

        return products;
    }

    private static Shop[] GetSeedShops()
    {
        return [
            new() { Street = "Олександра Поля", Building = "36", OpeningTime = "10:00", ClosingTime = "22:00" },
            new() { Street = "Незалежності", Building = "42", OpeningTime = "09:00", ClosingTime = "21:00" },
            new() { Street = "Дмитра Яворницького", Building = "12", OpeningTime = "10:00", ClosingTime = "21:00" },
            new() { Street = "Робоча", Building = "54", OpeningTime = "10:00", ClosingTime = "22:00" },
        ];
    }

    public static async Task SeedDatabaseAsync(StoreDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        if (!await context.UserRoles.AnyAsync())
        {
            List<UserRole> userRoles = [];
            foreach (var role in roles)
            {
                userRoles.Add(new() { Name = role });
            }

            await context.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
        }

        var adminExists = await context.Users.AnyAsync(u => u.UserRole!.Name == UserRoles.Admin);
        if (!adminExists)
        {
            var adminRole = await context.UserRoles.FirstOrDefaultAsync(r => r.Name == UserRoles.Admin);
            var hashedPassword = passwordHasher.Hash("AdminPassword1");
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

        if (!await context.ProductCategories.AnyAsync() && !await context.Products.AnyAsync())
        {
            context.Products.AddRange(GetSeedProducts());
            await context.SaveChangesAsync();
        }

        if (!await context.Shops.AnyAsync())
        {
            context.Shops.AddRange(GetSeedShops());
            await context.SaveChangesAsync();
        }
    }

    public static List<ProductCategory> GetTestCategories()
    {
        List<ProductCategory> categories = [
            new ProductCategory { Name = "Roll" },
            new ProductCategory { Name = "Pizza" },
            new ProductCategory { Name = "Food set" },
            new ProductCategory { Name = "Noodles" },
            new ProductCategory { Name = "Cold beverage" }];

        return categories;
    }

    public static List<Product> GetTestProducts()
    {
        var categories = GetTestCategories();

        List<Product> products = [
            new Product { Name = "California", Description = "desc", Price = 234, Category = categories[0] },
            new Product { Name = "Red Dragon", Description = "desc", Price = 197, Category = categories[0] },
            new Product { Name = "Pepperoni", Description = "desc", Price = 254, Category = categories[1] },
            new Product { Name = "Texas", Description = "desc", Price = 265, Category = categories[1] },
            new Product { Name = "Food set 1", Description = "desc", Price = 678, Category = categories[2] },
            new Product { Name = "Food set 2", Description = "desc", Price = 721, Category = categories[2] },
            new Product { Name = "Udon", Description = "desc", Price = 178, Category = categories[3] },
            new Product { Name = "Soba", Description = "desc", Price = 180, Category = categories[3] },
            new Product { Name = "Pepsi", Description = "desc", Price = 45, Category = categories[4] }];

        return products;
    }

    public static List<ProductCategory> GetServiceTestCategories()
    {
        List<ProductCategory> categories = [
            new ProductCategory { Id = 1, Name = "Roll" },
            new ProductCategory { Id = 2, Name = "Pizza" },
            new ProductCategory { Id = 3, Name = "Food set" },
            new ProductCategory { Id = 4, Name = "Noodles" },
            new ProductCategory { Id = 5, Name = "Cold beverage" }];

        return categories;
    }

    public static List<Product> GetServiceTestProducts()
    {
        var categories = GetServiceTestCategories();

        List<Product> products = [
            new Product { Id = 1, Name = "California", Description = "desc", Price = 234, Category = categories[0], CategoryId = categories[0].Id},
            new Product { Id = 2, Name = "Red Dragon", Description = "desc", Price = 197, Category = categories[0], CategoryId = categories[0].Id },
            new Product { Id = 3, Name = "Pepperoni", Description = "desc", Price = 254, Category = categories[1], CategoryId = categories[1].Id },
            new Product { Id = 4, Name = "Texas", Description = "desc", Price = 265, Category = categories[1], CategoryId = categories[1].Id },
            new Product { Id = 5, Name = "Food set 1", Description = "desc", Price = 678, Category = categories[2], CategoryId = categories[2].Id },
            new Product { Id = 6, Name = "Food set 2", Description = "desc", Price = 721, Category = categories[2], CategoryId = categories[2].Id },
            new Product { Id = 7, Name = "Udon", Description = "desc", Price = 178, Category = categories[3], CategoryId = categories[3].Id },
            new Product { Id = 8, Name = "Soba", Description = "desc", Price = 180, Category = categories[3], CategoryId = categories[3].Id },
            new Product { Id = 9, Name = "Pepsi", Description = "desc", Price = 45, Category = categories[4], CategoryId = categories[4].Id }];

        return products;
    }

    public static List<ProductCategoryModel> GetServiceTestCategoryModels()
    {
        List<ProductCategoryModel> categories = [
            new ProductCategoryModel { Id = 1, Name = "Roll" },
            new ProductCategoryModel { Id = 2, Name = "Pizza" },
            new ProductCategoryModel { Id = 3, Name = "Food set" },
            new ProductCategoryModel { Id = 4, Name = "Noodles" },
            new ProductCategoryModel { Id = 5, Name = "Cold beverage" }];

        return categories;
    }

    public static List<ProductModel> GetServiceTestProductModels()
    {
        var categories = GetServiceTestCategoryModels();

        List<ProductModel> products = [
            new ProductModel { Id = 1, Name = "California", Description = "desc", Price = 234, CategoryName = categories[0].Name, CategoryId = categories[0].Id },
            new ProductModel { Id = 2, Name = "Red Dragon", Description = "desc", Price = 197, CategoryName = categories[0].Name, CategoryId = categories[0].Id },
            new ProductModel { Id = 3, Name = "Pepperoni", Description = "desc", Price = 254, CategoryName = categories[1].Name, CategoryId = categories[1].Id },
            new ProductModel { Id = 4, Name = "Texas", Description = "desc", Price = 265, CategoryName = categories[1].Name, CategoryId = categories[1].Id },
            new ProductModel { Id = 5, Name = "Food set 1", Description = "desc", Price = 678, CategoryName = categories[2].Name, CategoryId = categories[2].Id },
            new ProductModel { Id = 6, Name = "Food set 2", Description = "desc", Price = 721, CategoryName = categories[2].Name, CategoryId = categories[2].Id },
            new ProductModel { Id = 7, Name = "Udon", Description = "desc", Price = 178, CategoryName = categories[3].Name, CategoryId = categories[3].Id },
            new ProductModel { Id = 8, Name = "Soba", Description = "desc", Price = 180, CategoryName = categories[3].Name, CategoryId = categories[3].Id },
            new ProductModel { Id = 9, Name = "Pepsi", Description = "desc", Price = 45, CategoryName = categories[4].Name, CategoryId = categories[4].Id }];

        return products;
    }
}
