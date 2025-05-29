using Domain.Abstractions.Services;
using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheOrderService : IOrderService
{
    private readonly IOrderService _orderService;
    private readonly ICacheService _cacheService;

    public CacheOrderService(IOrderService orderService, ICacheService cacheService)
    {
        _orderService = orderService;
        _cacheService = cacheService;
    }

    public async Task<OrderResponse> AddAsync(AddDeliveryOrderRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _orderService.AddAsync(request);
        await InvalidateAddCacheAsync();
        return response;
    }

    public async Task<OrderResponse> AddAsync(AddTakeawayOrderRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _orderService.AddAsync(request);
        await InvalidateAddCacheAsync();
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await _orderService.DeleteAsync(id);
        await _cacheService.RemoveByPrefixAsync("allOrders");
        await _cacheService.RemoveAsync($"orderById{id}");
    }

    public async Task<IList<OrderResponse>> GetAllAsync()
    {
        const string key = "allOrders";
        var cachedResponses = await _cacheService.GetAsync<IList<OrderResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _orderService.GetAllAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<IList<OrderResponse>> GetAllByUserIdAsync(long userId)
    {
        var key = $"allOrdersByUserId{userId}";
        var cachedResponses = await _cacheService.GetAsync<IList<OrderResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }

        var responses = await _orderService.GetAllByUserIdAsync(userId);
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<OrderResponse> GetByIdAsync(long id)
    {
        var key = $"orderById{id}";
        var cachedResponse = await _cacheService.GetAsync<OrderResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await _orderService.GetByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<OrderResponse> UpdateAsync(UpdateOrderRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _orderService.UpdateAsync(request);
        await _cacheService.RemoveByPrefixAsync("orderBy");
        await _cacheService.RemoveByPrefixAsync("allOrders");
        return response;
    }

    private async Task InvalidateAddCacheAsync()
    {
        await _cacheService.RemoveByPrefixAsync("allOrders");
    }
}