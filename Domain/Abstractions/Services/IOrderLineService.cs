using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Responses.OrderLines;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderLineService
{
    Task<IEnumerable<OrderLineResponse>> GetAllAsync(PageInfo? pageInfo = null);

    Task<OrderLineResponse?> GetByIdAsync(long id);

    Task<OrderLineResponse> AddAsync(AddOrderLineCommand command);

    Task<OrderLineResponse> UpdateAsync(UpdateOrderLineCommand command);

    Task DeleteAsync(long id);
    
    Task<IEnumerable<OrderLineResponse>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null);

    Task<OrderLineResponse?> GetByOrderAndProductIdsAsync(long orderId, long productId);
}
