using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Repositories;
using DokWokApiTests.EqualityComparers;
using Microsoft.EntityFrameworkCore;

namespace DokWokApiTests.DAL;

public class ProductRepositoryTests
{
    [Fact]
    public async Task ProductRepository_AddAsync_AddsValueToDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        var newEntity = new Product 
        { 
            Name = UnitTestHelper.GetRandomString(5, 15), Description = "TestDesc", CategoryId = 1, Price = 123 
        };

        // Act
        var oldLength = context.Products.Count();
        var result = await repository.AddAsync(newEntity);
        var newLength = context.Products.Count();

        // Assert
        Assert.Contains(result, context.Products, new ProductEqualityComparer());
        Assert.NotEqual(oldLength, newLength);

        // Clean up
        context.Remove(result);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task ProductRepository_DeleteAsync_DeletesValueFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        var entityToDelete = new Product 
        { 
            Name = UnitTestHelper.GetRandomString(5, 15), Description = "TestDesc", CategoryId = 1, Price = 123 
        };
        await context.AddAsync(entityToDelete);
        await context.SaveChangesAsync();

        // Act
        var oldLength = context.Products.Count();
        await repository.DeleteAsync(entityToDelete);
        var newLength = context.Products.Count();

        // Assert
        Assert.DoesNotContain(entityToDelete, context.Products, new ProductEqualityComparer());
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public async Task ProductRepository_DeleteByIdAsync_DeletesValueFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        var entityToDelete = new Product 
        { 
            Name = UnitTestHelper.GetRandomString(5, 15), Description = "TestDesc", CategoryId = 1, Price = 123 
        };
        await context.AddAsync(entityToDelete);
        await context.SaveChangesAsync();

        // Act
        var oldLength = context.Products.Count();
        await repository.DeleteByIdAsync(entityToDelete.Id);
        var newLength = context.Products.Count();

        // Assert
        Assert.DoesNotContain(entityToDelete, context.Products, new ProductEqualityComparer());
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public void ProductRepository_GetAll_GetsAllValuesFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        var expectedResult = context.Products;

        // Act
        var allEntities = repository.GetAll();

        // Assert
        Assert.Equal(expectedResult, allEntities);
    }

    [Fact]
    public void ProductRepository_GetAllWithDetails_GetsAllValuesFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        var expectedResult = context.Products.Include(p => p.Category);

        // Act
        var allEntities = repository.GetAllWithDetails();

        // Assert
        Assert.Equal(expectedResult, allEntities);
    }

    [Fact]
    public async Task ProductRepository_GetById_GetsValueByIdFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        long id = 1;
        var expectedResult = await context.Products.FindAsync(id);

        // Act
        var entitiy = await repository.GetByIdAsync(id);

        // Assert
        Assert.Equal(expectedResult, entitiy, new ProductEqualityComparer());
    }

    [Fact]
    public async Task ProductRepository_GetByIdWithDetails_GetsValueByIdFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        long id = 1;
        var expectedResult = await context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

        // Act
        var entitiy = await repository.GetByIdWithDetailsAsync(id);

        // Assert
        Assert.Equal(expectedResult, entitiy, new ProductEqualityComparer());
    }

    [Fact]
    public async Task ProductRepository_UpdateAsync_UpdatesValueFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductRepository(context);
        var entityToUpdate = new Product 
        { 
            Name = UnitTestHelper.GetRandomString(5, 15), Description = "TestDesc", CategoryId = 1, Price = 123 
        };
        await context.AddAsync(entityToUpdate);
        await context.SaveChangesAsync();
        context.Entry(entityToUpdate).State = EntityState.Detached;

        // Act
        var updatedEntity = new Product
        {
            Id = entityToUpdate.Id,
            Name = UnitTestHelper.GetRandomString(5, 15),
            Description = entityToUpdate.Description,
            CategoryId = entityToUpdate.CategoryId,
            Price = entityToUpdate.Price
        };
        var result = await repository.UpdateAsync(updatedEntity);

        // Assert
        Assert.DoesNotContain(entityToUpdate, context.Products, new ProductEqualityComparer());
        Assert.NotEqual(entityToUpdate, await context.Products.FindAsync(result.Id));

        // Clean up
        context.Remove(result);
        await context.SaveChangesAsync();
    }
}
