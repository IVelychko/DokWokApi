using DokWokApi.BLL.Models.Order;
using LanguageExt.Common;

namespace DokWokApi.BLL.Interfaces;

public interface IOrderService : ICrud<OrderModel>
{
    Task<IEnumerable<OrderModel>> GetAllByUserIdAsync(string userId);

    Task<Result<OrderModel>> AddOrderFromCartAsync(OrderModel model);
}
