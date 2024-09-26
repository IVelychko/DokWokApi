using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderService : ICrud<OrderModel>
{
    Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId, PageInfo? pageInfo = null);
}
