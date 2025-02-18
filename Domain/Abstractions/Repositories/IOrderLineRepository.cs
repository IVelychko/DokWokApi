using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<IList<OrderLine>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IList<OrderLine>> GetAllByOrderIdAsync(long orderId, PageInfo? pageInfo = null);

    Task<IList<OrderLine>> GetAllWithDetailsByOrderIdAsync(long orderId, PageInfo? pageInfo = null);

    Task<OrderLine?> GetByIdWithDetailsAsync(long id);

    Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId);

    Task<OrderLine?> GetByOrderAndProductIdsWithDetailsAsync(long orderId, long productId);
    
    Task<bool> AreOrderAndProductIdsUniqueAsync(long orderId, long productId, long orderLineIdToExclude);

    Task<bool> OrderLineExistsAsync(long id);
    
    Task<bool> OrderLineExistsAsync(long orderId, long productId);
}
