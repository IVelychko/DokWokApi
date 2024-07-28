using Application.Operations.Order.Commands.AddDeliveryOrder;
using Application.Operations.Order.Commands.AddTakeawayOrder;
using Application.Operations.Order.Commands.UpdateOrder;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class OrderMappingExtensions
{
    public static OrderModel ToModel(this AddDeliveryOrderRequest request)
    {
        return new()
        {
            DeliveryAddress = request.DeliveryAddress,
            CustomerName = request.CustomerName,
            Email = request.Email,
            PaymentType = request.PaymentType,
            PhoneNumber = request.PhoneNumber,
            UserId = request.UserId
        };
    }

    public static OrderModel ToModel(this AddTakeawayOrderRequest request)
    {
        return new()
        {
            ShopId = request.ShopId,
            CustomerName = request.CustomerName,
            Email = request.Email,
            PaymentType = request.PaymentType,
            PhoneNumber = request.PhoneNumber,
            UserId = request.UserId
        };
    }

    public static OrderModel ToModel(this UpdateOrderRequest request)
    {
        return new()
        {
            ShopId = request.ShopId,
            CustomerName = request.CustomerName,
            Email = request.Email,
            PaymentType = request.PaymentType,
            PhoneNumber = request.PhoneNumber,
            UserId = request.UserId,
            DeliveryAddress = request.DeliveryAddress,
            CreationDate = request.CreationDate,
            Id = request.Id,
            TotalOrderPrice = request.TotalOrderPrice,
            Status = request.Status
        };
    }
}
