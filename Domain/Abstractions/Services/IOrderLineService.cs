using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderLineService : ICrud<OrderLineModel>
{
    Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId);

    Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAndPageAsync(long orderId, int pageNumber, int pageSize);

    Task<OrderLineModel?> GetByOrderAndProductIdsAsync(long orderId, long productId);
}
