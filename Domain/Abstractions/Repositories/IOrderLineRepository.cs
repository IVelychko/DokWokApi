using Domain.Entities;
using Domain.Specifications.OrderLines;

namespace Domain.Abstractions.Repositories;

public interface IOrderLineRepository : IRepository<OrderLine>
{
    Task<IList<OrderLine>> GetAllBySpecificationAsync(OrderLineSpecification specification);
    
    Task<IList<OrderLine>> GetAllByOrderIdAsync(long orderId);

    Task<OrderLine?> GetBySpecificationAsync(OrderLineSpecification specification);

    Task<OrderLine?> GetByOrderAndProductIdsAsync(long orderId, long productId);
    
    Task<bool> AreOrderAndProductIdsUniqueAsync(long orderId, long productId, long orderLineIdToExclude);

    Task<bool> OrderLineExistsAsync(long id);
    
    Task<bool> OrderLineExistsAsync(long orderId, long productId);
}
