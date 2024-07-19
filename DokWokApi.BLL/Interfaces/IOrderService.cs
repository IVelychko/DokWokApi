using DokWokApi.BLL.Models.Order;

namespace DokWokApi.BLL.Interfaces;

public interface IOrderService : ICrud<OrderModel>
{
    Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId);
}
