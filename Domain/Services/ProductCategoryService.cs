using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Mapping.Extensions;
using Domain.Models;

namespace Domain.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _productCategoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public ProductCategoryService(IProductCategoryRepository productCategoryRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _productCategoryRepository = productCategoryRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<ProductCategoryModel>> AddAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productCategoryModel", "The passed model is null.");
            return Result<ProductCategoryModel>.Failure(error);
        }

        var entity = model.ToEntity();
        await _productCategoryRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        var createdEntity = await _productCategoryRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProductCategories");

        return createdEntity.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _productCategoryRepository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveAsync($"productCategoryById{id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProductCategories");
    }

    public async Task<IEnumerable<ProductCategoryModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allProductCategories" :
            $"paginatedAllProductCategories-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<ProductCategory> specification = new() { PageInfo = pageInfo };
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _productCategoryRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToModel());
    }

    public async Task<ProductCategoryModel?> GetByIdAsync(long id)
    {
        string key = $"productCategoryById{id}";
        Specification<ProductCategory> specification = new() { Criteria = c => c.Id == id };
        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _productCategoryRepository.GetAllBySpecificationAsync);

        return entity?.ToModel();
    }

    public async Task<Result<ProductCategoryModel>> UpdateAsync(ProductCategoryModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productCategoryModel", "The passed model is null.");
            return Result<ProductCategoryModel>.Failure(error);
        }

        var entity = model.ToEntity();
        _productCategoryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        var updatedEntity = await _productCategoryRepository.GetByIdAsync(entity.Id) ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveAsync($"productCategoryById{updatedEntity}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProductCategories");

        return updatedEntity.ToModel();
    }

    public async Task<Result<bool>> IsNameTakenAsync(string name)
    {
        if (name is null)
        {
            var error = new ValidationError(nameof(name), "The passed name is null");
            return Result<bool>.Failure(error);
        }

        var isTakenResult = await _productCategoryRepository.IsNameTakenAsync(name);
        return isTakenResult;
    }
}
