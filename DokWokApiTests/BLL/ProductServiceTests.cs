using DokWokApi.BLL.Models.Product;
using DokWokApi.BLL.Services;
using DokWokApi.DAL;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Repositories;
using DokWokApiTests.EqualityComparers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DokWokApiTests.BLL;

public class ProductServiceTests
{
    [Fact]
    public async Task ProductService_AddAsync_AddsValueToRepository()
    {
        // Arrange
        var entityList = UnitTestHelper.GetServiceTestProducts();
        var mapper = UnitTestHelper.GetMapper();
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns<Product>(product =>
            {
                entityList.Add(product);
                return Task.FromResult(product);
            });

        repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<long>()))
            .Returns<long>(id =>
            {
                var entity = entityList.Find(p => p.Id == id);
                if (entity is not null)
                {
                    entity.Category ??= UnitTestHelper.GetServiceTestCategories()[0];
                }
                return Task.FromResult(entity);
            });

        var service = new ProductService(repositoryMock.Object, mapper);

        // Act
        var oldLength = entityList.Count;
        var modelToAdd = new ProductModel
        {
            Id = 100,
            Name = UnitTestHelper.GetRandomString(5, 15),
            Description = "TestDesc",
            CategoryId = 1,
            Price = 234,
            CategoryName = UnitTestHelper.GetServiceTestCategories()[0].Name
        };
        var resultModel = await service.AddAsync(modelToAdd);
        var newLength = entityList.Count;

        // Assert
        Assert.Equal(modelToAdd, resultModel, new ProductModelEqualityComparer());
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public async Task ProductService_DeleteAsync_DeletesValueFromRepository()
    {
        // Arrange
        var entityList = UnitTestHelper.GetServiceTestProducts();
        var mapper = UnitTestHelper.GetMapper();
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.DeleteByIdAsync(It.IsAny<long>()))
            .Returns<long>(id =>
            {
                var entityToDelete = entityList.Find(e => e.Id == id);
                if (entityToDelete is not null)
                {
                    entityList.Remove(entityToDelete);
                }
                return Task.CompletedTask;
            });

        var service = new ProductService(repositoryMock.Object, mapper);

        // Act
        long id = 1;
        var oldLength = entityList.Count;
        await service.DeleteAsync(id);
        var newLength = entityList.Count;

        // Assert
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public async Task ProductService_GetAllAsync_GetsAllValuesFromRepository()
    {
        // Arrange
        using var context = new StoreDbContext(UnitTestHelper.GetDbContextOptions());
        var mapper = UnitTestHelper.GetMapper();
        var repository = new ProductRepository(context);
        var service = new ProductService(repository, mapper);
        var expectedResult = mapper.Map<IEnumerable<ProductModel>>(context.Products.Include(p => p.Category));

        // Act
        var resultList = await service.GetAllAsync();

        // Assert
        Assert.Equal(expectedResult, resultList, new ProductModelEqualityComparer());
    }

    [Fact]
    public async Task ProductService_GetByIdAsync_GetsValueByIdFromRepository()
    {
        // Arrange
        var entityList = UnitTestHelper.GetServiceTestProducts();
        var modelList = UnitTestHelper.GetServiceTestProductModels();
        var mapper = UnitTestHelper.GetMapper();
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<long>()))
            .Returns<long>(id =>
            {
                var entity = entityList.Find(p => p.Id == id);
                if (entity is not null)
                {
                    entity.Category ??= UnitTestHelper.GetServiceTestCategories()[0];
                }
                return Task.FromResult(entity);
            });

        var service = new ProductService(repositoryMock.Object, mapper);

        // Act
        long id = 1;
        var expected = modelList[0];
        var actual = await service.GetByIdAsync(id);
        
        // Assert
        Assert.Equal(expected, actual, new ProductModelEqualityComparer());
    }

    [Fact]
    public async Task ProductService_UpdateAsync_UpdatesValueFromRepository()
    {
        // Arrange
        var entityList = UnitTestHelper.GetServiceTestProducts();
        var mapper = UnitTestHelper.GetMapper();
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Returns<Product>(product =>
            {
                var entityToUpdate = entityList.Find(e => e.Id == product.Id);
                if (entityToUpdate is not null)
                {
                    entityToUpdate.Id = product.Id;
                    entityToUpdate.Category = product.Category;
                    entityToUpdate.Price = product.Price;
                    entityToUpdate.Name = product.Name;
                    entityToUpdate.CategoryId = product.CategoryId;
                    entityToUpdate.Description = product.Description;
                }
                return Task.FromResult(entityToUpdate ?? new Product());
            });

        repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<long>()))
            .Returns<long>(id =>
            {
                var entity = entityList.Find(p => p.Id == id);
                if (entity is not null)
                {
                    entity.Category ??= UnitTestHelper.GetServiceTestCategories()[0];
                }
                return Task.FromResult(entity);
            });

        var service = new ProductService(repositoryMock.Object, mapper);

        // Act
        var modelToUpdate = new ProductModel
        {
            Id = 1,
            Name = UnitTestHelper.GetRandomString(5, 15),
            Description = "TestDesc",
            CategoryId = 1,
            Price = 234,
            CategoryName = UnitTestHelper.GetServiceTestCategories()[0].Name
        };
        var result = await service.UpdateAsync(modelToUpdate);

        // Assert
        Assert.Equal(modelToUpdate, result, new ProductModelEqualityComparer());
    }
}
