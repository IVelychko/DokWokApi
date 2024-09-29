using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<Order>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null);

    Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(long userId, PageInfo? pageInfo = null);

    Task<Order?> GetByIdWithDetailsAsync(long id);
}
