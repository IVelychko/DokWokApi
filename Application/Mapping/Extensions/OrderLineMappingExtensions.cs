using Domain.DTOs.Commands.OrderLines;
using Domain.DTOs.Requests.OrderLines;
using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.OrderLines;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class OrderLineMappingExtensions
{
    public static AddOrderLineCommand ToCommand(this AddOrderLineRequest request) =>
        new(request.OrderId, request.ProductId, request.Quantity);

    public static UpdateOrderLineCommand ToCommand(this UpdateOrderLineRequest request) =>
        new(request.Id, request.OrderId, request.ProductId, request.Quantity);

    // public static OrderLineModel ToModel(this AddOrderLineCommand command)
    // {
    //     return new()
    //     {
    //         OrderId = command.OrderId,
    //         ProductId = command.ProductId,
    //         Quantity = command.Quantity
    //     };
    // }
    
    public static OrderLine ToEntity(this AddOrderLineCommand command)
    {
        return new OrderLine
        {
            OrderId = command.OrderId,
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
    }

    // public static OrderLineModel ToModel(this UpdateOrderLineCommand command)
    // {
    //     return new()
    //     {
    //         Id = command.Id,
    //         OrderId = command.OrderId,
    //         ProductId = command.ProductId,
    //         Quantity = command.Quantity
    //     };
    // }
    
    public static OrderLine ToEntity(this UpdateOrderLineCommand command)
    {
        return new OrderLine
        {
            Id = command.Id,
            OrderId = command.OrderId,
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
    }

    // public static OrderLineModel ToModel(this AddDeliveryOrderLineRequest request)
    // {
    //     return new()
    //     {
    //         ProductId = request.ProductId,
    //         Quantity = request.Quantity
    //     };
    // }
    
    public static OrderLine ToEntity(this AddDeliveryOrderLineRequest request)
    {
        return new OrderLine
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
        };
    }

    // public static OrderLineModel ToModel(this AddTakeawayOrderLineRequest request)
    // {
    //     return new()
    //     {
    //         ProductId = request.ProductId,
    //         Quantity = request.Quantity
    //     };
    // }
    
    public static OrderLine ToEntity(this AddTakeawayOrderLineRequest request)
    {
        return new OrderLine
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };
    }

    // public static OrderLineResponse ToResponse(this OrderLineModel model)
    // {
    //     return new()
    //     {
    //         Id = model.Id,
    //         OrderId = model.OrderId,
    //         ProductId = model.ProductId,
    //         Quantity = model.Quantity,
    //         TotalLinePrice = model.TotalLinePrice,
    //         Product = model.Product
    //     };
    // }
    
    public static OrderLineResponse ToResponse(this OrderLine entity)
    {
        return new OrderLineResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            ProductId = entity.ProductId,
            Quantity = entity.Quantity,
            TotalLinePrice = entity.TotalLinePrice,
            Product = entity.Product?.ToResponse()
        };
    }
}
