using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();

    Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(string userId);

    Task<Order?> GetByIdWithDetailsAsync(long id);
}
