using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Repositories;
using DokWokApi.DAL;
using DokWokApiTests.EqualityComparers;
using Microsoft.EntityFrameworkCore;

namespace DokWokApiTests.DAL;

public class ProductCategoryRepositoryTests
{
    [Fact]
    public async Task ProductCategoryRepository_AddAsync_AddsValueToDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductCategoryRepository(context);
        var newEntity = new ProductCategory { Name = UnitTestHelper.GetRandomString(5, 15) };

        // Act
        var oldLength = context.ProductCategories.Count();
        var result = await repository.AddAsync(newEntity);
        var newLength = context.ProductCategories.Count();

        // Assert
        Assert.Contains(result, context.ProductCategories, new ProductCategoryEqualityComparer());
        Assert.NotEqual(oldLength, newLength);

        // Clean up
        context.Remove(result);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task ProductCategoryRepository_DeleteAsync_DeletesValueFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductCategoryRepository(context);
        var entityToDelete = new ProductCategory { Name = UnitTestHelper.GetRandomString(5, 15) };
        await context.AddAsync(entityToDelete);
        await context.SaveChangesAsync();

        // Act
        var oldLength = context.ProductCategories.Count();
        await repository.DeleteAsync(entityToDelete);
        var newLength = context.ProductCategories.Count();

        // Assert
        Assert.DoesNotContain(entityToDelete, context.ProductCategories, new ProductCategoryEqualityComparer());
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public async Task ProductCategoryRepository_DeleteByIdAsync_DeletesValueFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductCategoryRepository(context);
        var entityToDelete = new ProductCategory { Name = UnitTestHelper.GetRandomString(5, 15) };
        await context.AddAsync(entityToDelete);
        await context.SaveChangesAsync();

        // Act
        var oldLength = context.ProductCategories.Count();
        await repository.DeleteByIdAsync(entityToDelete.Id);
        var newLength = context.ProductCategories.Count();

        // Assert
        Assert.DoesNotContain(entityToDelete, context.ProductCategories, new ProductCategoryEqualityComparer());
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public void ProductCategoryRepository_GetAll_GetsAllValuesFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductCategoryRepository(context);
        var expectedResult = context.ProductCategories;

        // Act
        var allEntities = repository.GetAll();

        // Assert
        Assert.Equal(expectedResult, allEntities);
    }

    [Fact]
    public async Task ProductCategoryRepository_GetById_GetsValueByIdFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductCategoryRepository(context);
        long id = 1;
        var expectedResult = await context.ProductCategories.FindAsync(id);

        // Act
        var entitiy = await repository.GetByIdAsync(id);

        // Assert
        Assert.Equal(expectedResult, entitiy, new ProductCategoryEqualityComparer());
    }

    [Fact]
    public async Task ProductCategoryRepository_UpdateAsync_UpdatesValueFromDb()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var repository = new ProductCategoryRepository(context);
        var entityToUpdate = new ProductCategory { Name = UnitTestHelper.GetRandomString(5, 15) };
        await context.AddAsync(entityToUpdate);
        await context.SaveChangesAsync();
        context.Entry(entityToUpdate).State = EntityState.Detached;

        // Act
        var updatedEntity = new ProductCategory
        {
            Id = entityToUpdate.Id,
            Name = UnitTestHelper.GetRandomString(5, 15)
        };
        var result = await repository.UpdateAsync(updatedEntity);

        // Assert
        Assert.DoesNotContain(entityToUpdate, context.ProductCategories, new ProductCategoryEqualityComparer());
        Assert.NotEqual(entityToUpdate, await context.ProductCategories.FindAsync(result.Id));

        // Clean up
        context.Remove(result);
        await context.SaveChangesAsync();
    }
}
