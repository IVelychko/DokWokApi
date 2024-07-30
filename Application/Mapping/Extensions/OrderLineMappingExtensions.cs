using Application.Operations.Order.Commands.AddDeliveryOrder;
using Application.Operations.Order.Commands.AddTakeawayOrder;
using Application.Operations.OrderLine;
using Application.Operations.OrderLine.Commands.AddOrderLine;
using Application.Operations.OrderLine.Commands.UpdateOrderLine;
using Domain.Models;

namespace Application.Mapping.Extensions;

public static class OrderLineMappingExtensions
{
    public static AddOrderLineCommand ToCommand(this AddOrderLineRequest request) =>
        new(request.OrderId, request.ProductId, request.Quantity);

    public static UpdateOrderLineCommand ToCommand(this UpdateOrderLineRequest request) =>
        new(request.Id, request.OrderId, request.ProductId, request.Quantity);

    public static OrderLineModel ToModel(this AddOrderLineCommand command)
    {
        return new()
        {
            OrderId = command.OrderId,
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
    }

    public static OrderLineModel ToModel(this UpdateOrderLineCommand command)
    {
        return new()
        {
            Id = command.Id,
            OrderId = command.OrderId,
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
    }

    public static OrderLineModel ToModel(this AddDeliveryOrderLineRequest request)
    {
        return new()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };
    }

    public static OrderLineModel ToModel(this AddTakeawayOrderLineRequest request)
    {
        return new()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };
    }

    public static OrderLineResponse ToResponse(this OrderLineModel model)
    {
        return new()
        {
            Id = model.Id,
            OrderId = model.OrderId,
            ProductId = model.ProductId,
            Quantity = model.Quantity,
            TotalLinePrice = model.TotalLinePrice,
            Product = model.Product
        };
    }
}
