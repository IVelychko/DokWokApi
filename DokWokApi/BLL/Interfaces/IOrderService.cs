using DokWokApi.BLL.Models.Order;

namespace DokWokApi.BLL.Interfaces;

public interface IOrderService
{
    Task<OrderModel> AddAsync(OrderForm model);

    Task DeleteAsync(long id);

    Task<IEnumerable<OrderModel>> GetAllAsync();

    Task<OrderModel?> GetByIdAsync(long id);
}
