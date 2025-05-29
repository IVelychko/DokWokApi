using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.Orders;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class OrderMappingExtensions
{
    
    public static Order ToEntity(this AddDeliveryOrderRequest command)
    {
        return new Order
        {
            DeliveryAddress = command.DeliveryAddress,
            CustomerName = command.CustomerName,
            Email = command.Email,
            PaymentType = command.PaymentType,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId,
            OrderLines = command.OrderLines.Select(r => r.ToEntity()).ToList()
        };
    }
    
    public static Order ToEntity(this AddTakeawayOrderRequest command)
    {
        return new Order
        {
            ShopId = command.ShopId,
            CustomerName = command.CustomerName,
            Email = command.Email,
            PaymentType = command.PaymentType,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId,
            OrderLines = command.OrderLines.Select(r => r.ToEntity()).ToList()
        };
    }
    
    public static Order ToEntity(this UpdateOrderRequest command)
    {
        return new Order
        {
            ShopId = command.ShopId,
            CustomerName = command.CustomerName,
            Email = command.Email,
            PaymentType = command.PaymentType,
            PhoneNumber = command.PhoneNumber,
            UserId = command.UserId,
            DeliveryAddress = command.DeliveryAddress,
            CreationDate = command.CreationDate,
            Id = command.Id,
            TotalOrderPrice = command.TotalOrderPrice,
            Status = command.Status
        };
    }
    
    public static OrderResponse ToResponse(this Order entity)
    {
        return new OrderResponse
        {
            Id = entity.Id,
            CreationDate = entity.CreationDate,
            CustomerName = entity.CustomerName,
            Email = entity.Email,
            PaymentType = entity.PaymentType,
            Status = entity.Status,
            TotalOrderPrice = entity.TotalOrderPrice,
            PhoneNumber = entity.PhoneNumber,
            DeliveryAddress = entity.DeliveryAddress,
            ShopId = entity.ShopId,
            UserId = entity.UserId,
            OrderLines = entity.OrderLines.Select(ol => ol.ToResponse()).ToList(),
        };
    }
}
