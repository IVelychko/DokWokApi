using Domain.Entities;
using Infrastructure.Validation.Orders.Add;
using Infrastructure.Validation.Orders.Update;

namespace Infrastructure.Mapping.Extensions;

public static class OrderMappingExtensions
{
    public static AddOrderValidationModel ToAddValidationModel(this Order order) =>
        new(order.CustomerName,
            order.PhoneNumber,
            order.Email,
            order.DeliveryAddress,
            order.PaymentType,
            order.TotalOrderPrice,
            order.CreationDate,
            order.Status,
            order.UserId,
            order.ShopId,
            order.OrderLines.ToList());

    public static UpdateOrderValidationModel ToUpdateValidationModel(this Order order) =>
        new(order.Id,
            order.CustomerName,
            order.PhoneNumber,
            order.Email,
            order.DeliveryAddress,
            order.PaymentType,
            order.TotalOrderPrice,
            order.CreationDate,
            order.Status,
            order.UserId,
            order.ShopId,
            order.OrderLines.ToList());
}
