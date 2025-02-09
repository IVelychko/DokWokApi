using Domain.DTOs.Commands.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAllAsync(PageInfo? pageInfo = null);

    Task<OrderResponse?> GetByIdAsync(long id);

    Task<OrderResponse> AddAsync(AddDeliveryOrderCommand command);

    Task<OrderResponse> AddAsync(AddTakeawayOrderCommand command);

    Task<OrderResponse> UpdateAsync(UpdateOrderCommand command);

    Task DeleteAsync(long id);
    
    Task<IEnumerable<OrderResponse>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null);
}
