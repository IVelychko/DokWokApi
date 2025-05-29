using Domain.Entities;
using Domain.Specifications.Orders;

namespace Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IList<Order>> GetAllBySpecificationAsync(OrderSpecification specification);
    
    Task<IList<Order>> GetAllByUserIdAsync(long userId);

    Task<Order?> GetBySpecificationAsync(OrderSpecification specification);
    
    Task<bool> OrderExistsAsync(long id);
}
