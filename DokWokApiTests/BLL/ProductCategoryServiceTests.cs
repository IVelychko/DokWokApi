using DokWokApi.BLL.Services;
using DokWokApi.DAL.Entities;
using DokWokApi.DAL.Interfaces;
using DokWokApi.DAL.Repositories;
using DokWokApi.DAL;
using DokWokApiTests.EqualityComparers;
using Moq;
using DokWokApi.BLL.Models.ProductCategory;

namespace DokWokApiTests.BLL;

public class ProductCategoryServiceTests
{
    [Fact]
    public async Task ProductCategoryService_AddAsync_AddsValueToRepository()
    {
        // Arrange
        var entityList = TestHelper.GetServiceTestCategories();
        var mapper = TestHelper.GetMapper();
        var repositoryMock = new Mock<IProductCategoryRepository>();

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<ProductCategory>()))
            .Returns<ProductCategory>(category =>
            {
                entityList.Add(category);
                return Task.FromResult(category);
            });

        var service = new ProductCategoryService(repositoryMock.Object, mapper);

        // Act
        var oldLength = entityList.Count;
        var modelToAdd = new ProductCategoryModel
        {
            Id = 100,
            Name = TestHelper.GetRandomString(5, 15)
        };
        var resultModel = await service.AddAsync(modelToAdd);
        var newLength = entityList.Count;

        // Assert
        Assert.Equal(modelToAdd, resultModel, new ProductCategoryModelEqualityComparer());
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public async Task ProductCategoryService_DeleteAsync_DeletesValueFromRepository()
    {
        // Arrange
        var entityList = TestHelper.GetServiceTestCategories();
        var mapper = TestHelper.GetMapper();
        var repositoryMock = new Mock<IProductCategoryRepository>();

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

        var service = new ProductCategoryService(repositoryMock.Object, mapper);

        // Act
        long id = 1;
        var oldLength = entityList.Count;
        await service.DeleteAsync(id);
        var newLength = entityList.Count;

        // Assert
        Assert.NotEqual(oldLength, newLength);
    }

    [Fact]
    public async Task ProductCategoryService_GetAllAsync_GetsAllValuesFromRepository()
    {
        // Arrange
        using var context = new StoreDbContext(TestHelper.GetDbContextOptions());
        var mapper = TestHelper.GetMapper();
        var repository = new ProductCategoryRepository(context);
        var service = new ProductCategoryService(repository, mapper);
        var expectedResult = mapper.Map<IEnumerable<ProductCategoryModel>>(context.ProductCategories);

        // Act
        var resultList = await service.GetAllAsync();

        // Assert
        Assert.Equal(expectedResult, resultList, new ProductCategoryModelEqualityComparer());
    }

    [Fact]
    public async Task ProductCategoryService_GetByIdAsync_GetsValueByIdFromRepository()
    {
        // Arrange
        var entityList = TestHelper.GetServiceTestCategories();
        var modelList = TestHelper.GetServiceTestCategoryModels();
        var mapper = TestHelper.GetMapper();
        var repositoryMock = new Mock<IProductCategoryRepository>();

        repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .Returns<long>(id =>
            {
                var entity = entityList.Find(p => p.Id == id);
                return Task.FromResult(entity);
            });

        var service = new ProductCategoryService(repositoryMock.Object, mapper);

        // Act
        long id = 1;
        var expected = modelList[0];
        var actual = await service.GetByIdAsync(id);

        // Assert
        Assert.Equal(expected, actual, new ProductCategoryModelEqualityComparer());
    }

    [Fact]
    public async Task ProductCategoryService_UpdateAsync_UpdatesValueFromRepository()
    {
        // Arrange
        var entityList = TestHelper.GetServiceTestCategories();
        var mapper = TestHelper.GetMapper();
        var repositoryMock = new Mock<IProductCategoryRepository>();

        repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ProductCategory>()))
            .Returns<ProductCategory>(category =>
            {
                var entityToUpdate = entityList.Find(e => e.Id == category.Id);
                if (entityToUpdate is not null)
                {
                    entityToUpdate.Id = category.Id;
                    entityToUpdate.Name = category.Name;
                }
                return Task.FromResult(entityToUpdate ?? new ProductCategory());
            });

        var service = new ProductCategoryService(repositoryMock.Object, mapper);

        // Act
        var modelToUpdate = new ProductCategoryModel
        {
            Id = 1,
            Name = TestHelper.GetRandomString(5, 15)
        };
        var result = await service.UpdateAsync(modelToUpdate);

        // Assert
        Assert.Equal(modelToUpdate, result, new ProductCategoryModelEqualityComparer());
    }
}
