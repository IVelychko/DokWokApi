using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();

    Task<IEnumerable<Order>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize);

    Task<IEnumerable<Order>> GetAllByUserIdAsync(string userId);

    Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAsync(string userId);

    Task<IEnumerable<Order>> GetAllByUserIdAndPageAsync(string userId, int pageNumber, int pageSize);

    Task<IEnumerable<Order>> GetAllWithDetailsByUserIdAndPageAsync(string userId, int pageNumber, int pageSize);

    Task<Order?> GetByIdWithDetailsAsync(long id);
}
