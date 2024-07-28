using Domain.Entities;
using Domain.Mapping.Extensions;
using Domain.Models;

namespace Domain.Mapping.Extensions;

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
