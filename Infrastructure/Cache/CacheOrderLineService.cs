using Domain.Abstractions.Services;
using Domain.DTOs.Requests.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Shared;

namespace Infrastructure.Cache;

public class CacheOrderLineService : IOrderLineService
{
    private readonly IOrderLineService _orderLineService;
    private readonly ICacheService _cacheService;
    
    public CacheOrderLineService(IOrderLineService orderLineService, ICacheService cacheService)
    {
        _orderLineService = orderLineService;
        _cacheService = cacheService;
    }

    public async Task<OrderLineResponse> AddAsync(AddOrderLineRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _orderLineService.AddAsync(request);
        await _cacheService.RemoveAsync($"allOrderLinesByOrderId{response.OrderId}");
        await _cacheService.RemoveAsync("allOrderLines");
        return response;
    }

    public async Task DeleteAsync(long id)
    {
        await _orderLineService.DeleteAsync(id);
        await _cacheService.RemoveByPrefixAsync("allOrderLines");
        await _cacheService.RemoveByPrefixAsync("orderLine");
    }

    public async Task<IList<OrderLineResponse>> GetAllAsync()
    {
        const string key = "allOrderLines";
        var cachedResponses = await _cacheService.GetAsync<IList<OrderLineResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }
        
        var responses = await _orderLineService.GetAllAsync();
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<IList<OrderLineResponse>> GetAllByOrderIdAsync(long orderId)
    {
        var key = $"allOrderLinesByOrderId{orderId}";
        var cachedResponses = await _cacheService.GetAsync<IList<OrderLineResponse>>(key);
        if (cachedResponses is not null)
        {
            return cachedResponses;
        }
        
        var responses = await _orderLineService.GetAllByOrderIdAsync(orderId);
        await _cacheService.SetAsync(key, responses);
        return responses;
    }

    public async Task<OrderLineResponse> GetByIdAsync(long id)
    {
        var key = $"orderLineById{id}";
        var cachedResponse = await _cacheService.GetAsync<OrderLineResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }
        
        var response = await _orderLineService.GetByIdAsync(id);
        await _cacheService.SetAsync(key, response);
        return response;
    }
    
    public async Task<OrderLineResponse> GetByOrderAndProductIdsAsync(long orderId, long productId)
    {
        var key = $"orderLineByOrderId{orderId}-ProductId{productId}";
        var cachedResponse = await _cacheService.GetAsync<OrderLineResponse>(key);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }
        
        var response = await _orderLineService.GetByOrderAndProductIdsAsync(orderId, productId);
        await _cacheService.SetAsync(key, response);
        return response;
    }

    public async Task<OrderLineResponse> UpdateAsync(UpdateOrderLineRequest request)
    {
        Ensure.ArgumentNotNull(request);
        var response = await _orderLineService.UpdateAsync(request);
        await _cacheService.RemoveByPrefixAsync("allOrderLinesByOrderId");
        await _cacheService.RemoveByPrefixAsync("orderLineByOrderId");
        await _cacheService.RemoveAsync("allOrderLines");
        await _cacheService.RemoveAsync($"orderLineById{response.Id}");
        return response;
    }
}