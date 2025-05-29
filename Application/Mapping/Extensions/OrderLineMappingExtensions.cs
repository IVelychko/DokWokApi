using Domain.DTOs.Requests.OrderLines;
using Domain.DTOs.Requests.Orders;
using Domain.DTOs.Responses.OrderLines;
using Domain.Entities;

namespace Application.Mapping.Extensions;

public static class OrderLineMappingExtensions
{
    public static OrderLine ToEntity(this AddOrderLineRequest command)
    {
        return new OrderLine
        {
            OrderId = command.OrderId,
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
    }
    
    public static OrderLine ToEntity(this UpdateOrderLineRequest command)
    {
        return new OrderLine
        {
            Id = command.Id,
            OrderId = command.OrderId,
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
    }
    
    public static OrderLine ToEntity(this AddDeliveryOrderLineRequest request)
    {
        return new OrderLine
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
        };
    }
    
    public static OrderLine ToEntity(this AddTakeawayOrderLineRequest request)
    {
        return new OrderLine
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };
    }
    
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
