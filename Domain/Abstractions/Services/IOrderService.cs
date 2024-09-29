using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IOrderService : ICrud<OrderModel>
{
    Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(long userId, PageInfo? pageInfo = null);
}
