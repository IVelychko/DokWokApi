using DokWokApi.BLL.Models.Order;
using DokWokApi.DAL.Entities;

namespace DokWokApi.BLL.Extensions;

public static class OrderLineMappingExtensions
{
    public static OrderLineModel ToModel(this OrderLine entity)
    {
        return new()
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ProductId = entity.ProductId,
            Quantity = entity.Quantity,
            TotalLinePrice = entity.TotalLinePrice,
            Product = entity.Product?.ToModel()
        };
    }

    public static OrderLineModel ToModel(this OrderLinePostModel model)
    {
        return new()
        {
            OrderId = model.OrderId!.Value,
            ProductId = model.ProductId!.Value,
            Quantity = model.Quantity!.Value
        };
    }

    public static OrderLineModel ToModel(this OrderLinePutModel model)
    {
        return new()
        {
            Id = model.Id!.Value,
            OrderId = model.OrderId!.Value,
            ProductId = model.ProductId!.Value,
            Quantity = model.Quantity!.Value
        };
    }

    public static OrderLine ToEntity(this OrderLineModel model)
    {
        return new()
        {
            Id = model.Id,
            OrderId = model.OrderId,
            ProductId = model.ProductId,
            Quantity = model.Quantity,
            TotalLinePrice = model.TotalLinePrice
        };
    }
}
