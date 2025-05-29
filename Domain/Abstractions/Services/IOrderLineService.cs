using Domain.DTOs.Requests.OrderLines;
using Domain.DTOs.Responses.OrderLines;

namespace Domain.Abstractions.Services;

public interface IOrderLineService
{
    Task<IList<OrderLineResponse>> GetAllAsync();

    Task<OrderLineResponse> GetByIdAsync(long id);

    Task<OrderLineResponse> AddAsync(AddOrderLineRequest request);

    Task<OrderLineResponse> UpdateAsync(UpdateOrderLineRequest request);

    Task DeleteAsync(long id);
    
    Task<IList<OrderLineResponse>> GetAllByOrderIdAsync(long orderId);

    Task<OrderLineResponse> GetByOrderAndProductIdsAsync(long orderId, long productId);
}
