using DokWokApi.BLL;
using DokWokApi.BLL.Interfaces;
using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Exceptions;
using DokWokApi.DAL.ResultType;
using DokWokApi.Services;

namespace DokWokApi.Extensions;

public static class OrderServiceExtensions
{
    public static async Task<Result<OrderModel>> AddOrderFromCartAsync(this IOrderService orderService, 
        OrderModel model, ICartService cartService)
    {
        if (model is null)
        {
            var exception = new ValidationException("The passed model is null.");
            return new Result<OrderModel>(exception);
        }

        var cart = await cartService.GetCart();
        if (cart is null)
        {
            var exception = new CartException("There was an error while getting the cart.");
            return new Result<OrderModel>(exception);
        }
        
        if (cart.Lines.Count < 1)
        {
            var exception = new CartException("There are no products in the cart");
            return new Result<OrderModel>(exception);
        }

        var orderLines = cart.Lines.Select(cl => cl.ToOrderLineModel()).ToList();
        model.CreationDate = DateTime.UtcNow;
        model.OrderLines = orderLines;
        model.TotalOrderPrice = cart.TotalCartPrice;
        model.Status = OrderStatuses.BeingProcessed;

        var addedModel = await orderService.AddAsync(model);
        await cartService.ClearCart();
        return addedModel;
    }
}
