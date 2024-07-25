using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL.Extensions;

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

    public static OrderModel ToModel(this DeliveryOrderModel model)
    {
        return new()
        {
            DeliveryAddress = model.DeliveryAddress,
            CustomerName = model.CustomerName!,
            Email = model.Email!,
            PaymentType = model.PaymentType!,
            PhoneNumber = model.PhoneNumber!,
            UserId = model.UserId
        };
    }

    public static OrderModel ToModel(this TakeawayOrderModel model)
    {
        return new()
        {
            ShopId = model.ShopId,
            CustomerName = model.CustomerName!,
            Email = model.Email!,
            PaymentType = model.PaymentType!,
            PhoneNumber = model.PhoneNumber!,
            UserId = model.UserId
        };
    }

    public static OrderModel ToModel(this OrderPutModel model)
    {
        return new()
        {
            ShopId = model.ShopId,
            CustomerName = model.CustomerName!,
            Email = model.Email!,
            PaymentType = model.PaymentType!,
            PhoneNumber = model.PhoneNumber!,
            UserId = model.UserId,
            DeliveryAddress = model.DeliveryAddress,
            CreationDate = model.CreationDate!.Value,
            Id = model.Id!.Value,
            TotalOrderPrice = model.TotalOrderPrice!.Value,
            Status = model.Status!
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
