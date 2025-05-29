using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.Orders;

namespace Domain.Abstractions.Services;

public interface IOrderService
{
    Task<IList<OrderResponse>> GetAllAsync();

    Task<OrderResponse> GetByIdAsync(long id);

    Task<OrderResponse> AddAsync(AddDeliveryOrderRequest request);

    Task<OrderResponse> AddAsync(AddTakeawayOrderRequest request);

    Task<OrderResponse> UpdateAsync(UpdateOrderRequest request);

    Task DeleteAsync(long id);
    
    Task<IList<OrderResponse>> GetAllByUserIdAsync(long userId);
}
