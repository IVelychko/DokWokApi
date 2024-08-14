using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync();

    Task<IEnumerable<OrderLine>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize);

    Task<IEnumerable<OrderLine>> GetAllByOrderIdAsync(long orderId);

    Task<IEnumerable<OrderLine>> GetAllByOrderIdAndPageAsync(long orderId, int pageNumber, int pageSize);

    Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId);

    Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAndPageAsync(long orderId, int pageNumber, int pageSize);

    Task<OrderLine?> GetByIdWithDetailsAsync(long id);

    Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId);

    Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId);
}
