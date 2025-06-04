using Domain.Abstractions.Services;
using Domain.DTOs.Requests.Shops;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Shops;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheShopService : IShopService
{
    private readonly IShopService _shopService;
    private readonly ICacheService _cacheService;

    public CacheShopService(IShopService shopService, ICacheService cacheService)
    {
        _shopService = shopService;
        _cacheService = cacheService;
    }

    public async Task<ShopResponse> AddAsync(AddShopRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _shopService.AddAsync(request);
        await _cacheService.RemoveAsync("allShops");
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await _shopService.DeleteAsync(id);
        await _cacheService.RemoveAsync("allShops");
        await _cacheService.RemoveByPrefixAsync("shopBy");
    }

    public async Task<IList<ShopResponse>> GetAllAsync()
    {
        const string key = "allShops";
        var cachedResponses = await _cacheService.GetAsync<IList<ShopResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _shopService.GetAllAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<ShopResponse> GetByIdAsync(long id)
    {
        var key = $"shopById{id}";
        var cachedResponse = await _cacheService.GetAsync<ShopResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _shopService.GetByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<ShopResponse> GetByAddressAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        var key = $"shopByAddress-{street}-{building}";
        var cachedResponse = await _cacheService.GetAsync<ShopResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _shopService.GetByAddressAsync(street, building);
        await _cacheService.SetAsync(key, response);
        return response;
    }
    
    public async Task<IsTakenResponse> IsAddressTakenAsync(string street, string building)
    {
        Ensure.ArgumentNotNullOrWhiteSpace(street, nameof(street));
        Ensure.ArgumentNotNullOrWhiteSpace(building, nameof(building));
        return await _shopService.IsAddressTakenAsync(street, building);
    }

    public async Task<ShopResponse> UpdateAsync(UpdateShopRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _shopService.UpdateAsync(request);
        await _cacheService.RemoveByPrefixAsync("shopBy");
        await _cacheService.RemoveAsync("allShops");
        return response;
    }
}