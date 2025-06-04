using Domain.Abstractions.Services;
using Domain.DTOs.Requests.ProductCategories;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryService _productCategoryService;
    private readonly ICacheService _cacheService;

    public CacheProductCategoryService(IProductCategoryService productCategoryService, ICacheService cacheService)
    {
        _productCategoryService = productCategoryService;
        _cacheService = cacheService;
    }

    public async Task<ProductCategoryResponse> AddAsync(AddProductCategoryRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _productCategoryService.AddAsync(request);
        await _cacheService.RemoveAsync("allProductCategories");
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await _productCategoryService.DeleteAsync(id);
        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveAsync($"productCategoryById{id}");
    }

    public async Task<IList<ProductCategoryResponse>> GetAllAsync()
    {
        const string key = "allProductCategories";
        var cachedResponses = await _cacheService.GetAsync<IList<ProductCategoryResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _productCategoryService.GetAllAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<ProductCategoryResponse> GetByIdAsync(long id)
    {
        var key = $"productCategoryById{id}";
        var cachedResponse = await _cacheService.GetAsync<ProductCategoryResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _productCategoryService.GetByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<ProductCategoryResponse> UpdateAsync(UpdateProductCategoryRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _productCategoryService.UpdateAsync(request);
        await _cacheService.RemoveAsync("allProductCategories");
        await _cacheService.RemoveByPrefixAsync("productCategory");
        return response;
    }

    public async Task<IsTakenResponse> IsNameTakenAsync(string name)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(name, nameof(name));
        return await _productCategoryService.IsNameTakenAsync(name);
    }
}