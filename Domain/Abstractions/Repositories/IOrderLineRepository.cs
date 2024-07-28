using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync();

    Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId);

    Task<OrderLine?> GetByIdWithDetailsAsync(long id);

    Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId);

    Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId);
}
