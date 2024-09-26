using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<IEnumerable<OrderLine>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<OrderLine>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null);

    Task<IEnumerable<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId, PageInfo? pageInfo = null);

    Task<OrderLine?> GetByIdWithDetailsAsync(long id);

    Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId);

    Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId);
}
