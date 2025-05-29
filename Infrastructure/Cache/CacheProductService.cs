using Domain.Abstractions.Services;
using Domain.DTOs.Requests.Products;
using Domain.DTOs.Responses.Products;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheProductService : IProductService
{
    private readonly IProductService _productService;
    private readonly ICacheService _cacheService;

    public CacheProductService(IProductService productService, ICacheService cacheService)
    {
        _productService = productService;
        _cacheService = cacheService;
    }

    public async Task<ProductResponse> AddAsync(AddProductRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _productService.AddAsync(request);
        await _cacheService.RemoveByPrefixAsync("allProducts");
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await _productService.DeleteAsync(id);
        await _cacheService.RemoveByPrefixAsync("allProducts");
        await _cacheService.RemoveAsync($"productById{id}");
    }

    public async Task<IList<ProductResponse>> GetAllAsync()
    {
        const string key = "allProducts";
        var cachedResponses = await _cacheService.GetAsync<IList<ProductResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _productService.GetAllAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<IList<ProductResponse>> GetAllByCategoryIdAsync(long categoryId)
    {
        var key = $"allProductsByCategoryId{categoryId}";
        var cachedResponses = await _cacheService.GetAsync<IList<ProductResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _productService.GetAllByCategoryIdAsync(categoryId);
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<ProductResponse> GetByIdAsync(long id)
    {
        var key = $"productById{id}";
        var cachedResponse = await _cacheService.GetAsync<ProductResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _productService.GetByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<ProductResponse> UpdateAsync(UpdateProductRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _productService.UpdateAsync(request);
        await _cacheService.RemoveByPrefixAsync("allProducts");
        await _cacheService.RemoveByPrefixAsync("productById");
        return response;
    }
    
    public async Task<bool> IsNameUniqueAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        return await _productService.IsNameUniqueAsync(name);
    }
}