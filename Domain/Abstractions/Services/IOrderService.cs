using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderService : ICrud<OrderModel>
{
    Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId);

    Task<IEnumerable<OrderModel>> GetAllByUserIdAndPageAsync(string userId, int pageNumber, int pageSize);
}
