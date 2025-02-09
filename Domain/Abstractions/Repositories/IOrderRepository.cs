using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IList<Order>> GetAllWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IList<Order>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null);

    Task<IList<Order>> GetAllWithDetailsByUserIdAsync(long userId, PageInfo? pageInfo = null);

    Task<Order?> GetByIdWithDetailsAsync(long id);
}
