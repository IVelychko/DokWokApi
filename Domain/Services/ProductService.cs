using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Errors;
using Domain.Mapping.Extensions;
using Domain.Models;
using Domain.ResultType;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Domain.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IDistributedCache _distributedCache;

    public ProductService(IProductRepository productRepository, IDistributedCache distributedCache)
    {
        _productRepository = productRepository;
        _distributedCache = distributedCache;
    }

    public async Task<Result<ProductModel>> AddAsync(ProductModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productModel", "The passed model is null.");
            return Result<ProductModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _productRepository.AddAsync(entity);
        return result.Match(p => p.ToModel(), Result<ProductModel>.Failure);
    }

    public async Task<bool?> DeleteAsync(long id)
    {
        return await _productRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<ProductModel>> GetAllAsync(PageInfo? pageInfo = null)
    {
        string key = "allProducts";
        var cachedSerializedModels = await _distributedCache.GetStringAsync(key);
        IEnumerable<ProductModel> models;
        if (string.IsNullOrEmpty(cachedSerializedModels))
        {
            var entities = await _productRepository.GetAllWithDetailsAsync(pageInfo);
            models = entities.Select(p => p.ToModel());
            if (models.Any())
            {
                await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(models));
                return models;
            }

            return models;
        }

        models = JsonSerializer.Deserialize<IEnumerable<ProductModel>>(cachedSerializedModels)!;
        return models;
    }

    public async Task<IEnumerable<ProductModel>> GetAllByCategoryIdAsync(long categoryId, PageInfo? pageInfo = null)
    {
        var entities = await _productRepository.GetAllWithDetailsByCategoryIdAsync(categoryId, pageInfo);
        var models = entities.Select(p => p.ToModel());
        return models;
    }

    public async Task<ProductModel?> GetByIdAsync(long id)
    {
        var entity = await _productRepository.GetByIdWithDetailsAsync(id);
        return entity?.ToModel();
    }

    public async Task<Result<ProductModel>> UpdateAsync(ProductModel model)
    {
        if (model is null)
        {
            var error = new ValidationError("productModel", "The passed model is null.");
            return Result<ProductModel>.Failure(error);
        }

        var entity = model.ToEntity();
        var result = await _productRepository.UpdateAsync(entity);
        return result.Match(p => p.ToModel(), Result<ProductModel>.Failure);
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
