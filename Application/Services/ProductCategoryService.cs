using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Shared;

namespace Application.Services;

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

    public async Task<ProductCategoryResponse> AddAsync(AddProductCategoryCommand command)
    {
        Ensure.ArgumentNotNull(command);
        ProductCategory entity = command.ToEntity();
        await _productCategoryRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        ProductCategory createdEntity = await _productCategoryRepository.GetByIdAsync(entity.Id) 
                                        ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProductCategories");

        return createdEntity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        ProductCategory? entity = await _productCategoryRepository.GetByIdAsync(id);
        entity = Ensure.EntityFound(entity, "The category was not found");
        _productCategoryRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveAsync($"productCategoryById{id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProductCategories");
    }

    public async Task<IEnumerable<ProductCategoryResponse>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allProductCategories" :
            $"paginatedAllProductCategories-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<ProductCategory> specification = new() { PageInfo = pageInfo };
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _productCategoryRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<ProductCategoryResponse?> GetByIdAsync(long id)
    {
        string key = $"productCategoryById{id}";
        Specification<ProductCategory> specification = new() { Criteria = c => c.Id == id };
        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _productCategoryRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<ProductCategoryResponse> UpdateAsync(UpdateProductCategoryCommand command)
    {
        Ensure.ArgumentNotNull(command);
        ProductCategory entity = command.ToEntity();
        _productCategoryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        ProductCategory updatedEntity = await _productCategoryRepository.GetByIdAsync(entity.Id) 
                                        ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveAsync($"productCategoryById{updatedEntity.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProductCategories");

        return updatedEntity.ToResponse();
    }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name);
        return await _productCategoryRepository.IsNameUniqueAsync(name);
    }
}
