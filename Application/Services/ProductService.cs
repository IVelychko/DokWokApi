using Application.Mapping.Extensions;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Products;
using Domain.DTOs.Responses.Products;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Shared;

namespace Application.Services;

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

    public async Task<ProductResponse> AddAsync(AddProductCommand command)
    {
        Ensure.ArgumentNotNull(command);
        Product entity = command.ToEntity();
        await _productRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        Product createdEntity = await _productRepository.GetByIdWithDetailsAsync(entity.Id) 
                                ?? throw new DbException("There was a database error");

        await _cacheService.RemoveAsync("allProducts");
        await _cacheService.RemoveAsync($"allProductsByCategoryId{createdEntity.CategoryId}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProducts");

        return createdEntity.ToResponse();
    }

    public async Task DeleteAsync(long id)
    {
        Product? entityToDelete = await _productRepository.GetByIdAsync(id);
        entityToDelete = Ensure.EntityFound(entityToDelete, "The product was not found");
        _productRepository.Delete(entityToDelete);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync("allProducts");
        await _cacheService.RemoveAsync($"allProductsByCategoryId{entityToDelete.CategoryId}");
        await _cacheService.RemoveAsync($"productById{id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProducts");
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = pageInfo is null ? "allProducts" :
            $"paginatedAllProducts-page{pageInfo.Number}-size{pageInfo.Size}";

        Specification<Product> specification = new() { PageInfo = pageInfo };
        specification.IncludeExpressions.Add(new(p => p.Category));
        var entities = await Caching.GetCollectionFromCache(_cacheService,
            key, specification, _productRepository.GetAllBySpecificationAsync);

        return entities.Select(p => p.ToResponse());
    }

    public async Task<IEnumerable<ProductResponse>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
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

        return entities.Select(p => p.ToResponse());
    }

    public async Task<ProductResponse?> GetByIdAsync(long id)
    {
        string key = $"productById{id}";
        Specification<Product> specification = new() { Criteria = p => p.Id == id };
        specification.IncludeExpressions.Add(new(p => p.Category));

        var entity = await Caching.GetEntityFromCache(_cacheService,
            key, specification, _productRepository.GetAllBySpecificationAsync);

        return entity?.ToResponse();
    }

    public async Task<ProductResponse> UpdateAsync(UpdateProductCommand command)
    {
        Ensure.ArgumentNotNull(command);
        Product entity = command.ToEntity();
        _productRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        
        Product updatedEntity = await _productRepository.GetByIdWithDetailsAsync(entity.Id) 
                                ?? throw new DbException("There was a database error");
        await _cacheService.RemoveByPrefixAsync("allProductsByCategoryId");
        await _cacheService.RemoveAsync("allProducts");
        await _cacheService.RemoveAsync($"productById{entity.Id}");
        await _cacheService.RemoveByPrefixAsync("paginatedAllProducts");

        return updatedEntity.ToResponse();
    }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNull(name);
        return await _productRepository.IsNameUniqueAsync(name);
    }
}
