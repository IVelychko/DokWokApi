using DokWokApi.BLL.Models.Order;

namespace DokWokApi.BLL.Interfaces;

public interface IOrderLineService : ICrud<OrderLineModel>
{
    Task<IEnumerable<OrderLineModel>> GetAllByOrderIdAsync(long orderId);
}
