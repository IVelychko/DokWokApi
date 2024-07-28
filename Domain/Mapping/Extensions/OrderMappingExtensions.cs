using Domain.Entities;
using Domain.Models;

namespace Domain.Mapping.Extensions;

public static class OrderMappingExtensions
{
    public static OrderModel ToModel(this Order entity)
    {
        return new()
        {
            Id = entity.Id,
            DeliveryAddress = entity.DeliveryAddress,
            CreationDate = entity.CreationDate,
            CustomerName = entity.CustomerName,
            Email = entity.Email,
            PaymentType = entity.PaymentType,
            PhoneNumber = entity.PhoneNumber,
            ShopId = entity.ShopId,
            Status = entity.Status,
            UserId = entity.UserId,
            TotalOrderPrice = entity.TotalOrderPrice,
            OrderLines = entity.OrderLines.Select(ol => ol.ToModel()).ToList()
        };
    }

    public static Order ToEntity(this OrderModel model)
    {
        return new()
        {
            Id = model.Id,
            DeliveryAddress = model.DeliveryAddress,
            CreationDate = model.CreationDate,
            CustomerName = model.CustomerName,
            Email = model.Email,
            PaymentType = model.PaymentType,
            PhoneNumber = model.PhoneNumber,
            ShopId = model.ShopId,
            Status = model.Status,
            UserId = model.UserId,
            TotalOrderPrice = model.TotalOrderPrice,
            OrderLines = model.OrderLines.Select(olm => olm.ToEntity()).ToList()
        };
    }
}
