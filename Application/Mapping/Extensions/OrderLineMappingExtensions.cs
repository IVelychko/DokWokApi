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
}
