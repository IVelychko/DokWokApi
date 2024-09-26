using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderLineService : ICrud<OrderLineModel>
{
    Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null);

    Task<OrderLineModel?> GetByOrderAndProductIdsAsync(long orderId, long productId);
}
