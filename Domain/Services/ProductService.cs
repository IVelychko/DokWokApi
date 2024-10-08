using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;

namespace Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public ProductService(IProductRepository productRepository, ICacheService cacheService, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductModel>> AddAsync(ProductModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productModel", "The passed model is null.");
            return Result<ProductModel>.Failure(error);
        }

        var entity = model.ToEntity();
        await _productRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _productRepository.GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allProducts");
        await _cacheService.RemoveAsync($"allProductsByCategoryId{createdEntity.CategoryId}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProducts");

        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        var entityToDelete = await _productRepository.GetByIdAsync(id) ?? throw new DbException("There was a database error");

        await _productRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allProducts");
        await _cacheService.RemoveAsync($"allProductsByCategoryId{entityToDelete.CategoryId}");
        await _cacheService.RemoveAsync($"productById{id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProducts");
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allProducts" :
            $"paginatedAllProducts-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Product> specification = new() { PageInfo = pageInfo };
        specification.IncludeExpressions.Add(new(p => p.Category));
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _productRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToModel());
    }

    public async Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? $"allProductsByCategoryId{categoryId}" :
            $"paginatedAllProductsByCategoryId{categoryId}-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Product> specification = new()
        {
            Criteria = p => p.CategoryId == categoryId,
            PageInfo = pageInfo
        };
        specification.IncludeExpressions.Add(new(p => p.Category));
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _productRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToModel());
    }

    public async Task<ProductModel?> GetByIdAsync(long id)
    {
        string key = $"productById{id}";
        Specification<Product> specification = new() { Criteria = p => p.Id == id };
        specification.IncludeExpressions.Add(new(p => p.Category));

        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _productRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<Result<ProductModel>> UpdateAsync(ProductModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productModel", "The passed model is null.");
            return Result<ProductModel>.Failure(error);
        }

        var entityToUpdate = await _productRepository.GetByIdAsync(model.Id) ?? throw new DbException("There was a database error");
        await _cacheService.RemoveAsync($"allProductsByCategoryId{entityToUpdate.CategoryId}");
        await _cacheService.RemoveAsync("allProducts");
        await _cacheService.RemoveAsync($"productById{entityToUpdate.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProducts");

        var entity = model.ToEntity();
        _productRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _productRepository.GetByIdWithDetailsAsync(entity.Id) ?? throw new DbException("There was a database error");

        return updatedEntity.ToModel();
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (name is null)
        {
            var error = new ValidationError(nameof(name), "The passed name is null");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _productRepository.IsNameTakenAsync(name);
        return isTakenResult;
    }
}
