using DokWokApi.BLL.Models;

namespace DokWokApi.BLL.Interfaces;

public interface IOrderService
{
    Task<OrderModel> AddAsync(OrderForm model);
}
