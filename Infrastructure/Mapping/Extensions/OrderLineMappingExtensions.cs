using Domain.Entities;
using Infrastructure.Validation.OrderLines.Add;
using Infrastructure.Validation.OrderLines.Update;

namespace Infrastructure.Mapping.Extensions;

public static class OrderLineMappingExtensions
{
    public static AddOrderLineValidationModel ToAddValidationModel(this OrderLine orderLine) =>
        new(orderLine.OrderId, orderLine.ProductId, orderLine.Quantity);

    public static UpdateOrderLineValidationModel ToUpdateValidationModel(this OrderLine orderLine) =>
        new(orderLine.Id, orderLine.OrderId, orderLine.ProductId, orderLine.Quantity);
}
