using AutoMapper;
using DokWokApi.BLL;
using DokWokApi.BLL.Models;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DokWokApiTests;

public static class UnitTestHelper
{
    public static IMapper GetMapper()
    {
        var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AutomapperProfile()));
        IMapper mapper = mapperConfig.CreateMapper();

        return mapper;
    }

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
                .UseSqlServer("Server=femuspc;Database=FoodStore;MultipleActiveResultSets=true;Trusted_Connection=true;TrustServerCertificate=true;")
                .Options;

        using (var context = new StoreDbContext(options))
        {
            SeedDatabase(context);
        }

        return options;
    }

    private static void SeedDatabase(StoreDbContext context)
    {
        if (!context.ProductCategories.Any() && !context.Products.Any())
        {
            context.Products.AddRange(GetTestProducts());
            context.SaveChanges();
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
