using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public sealed record UpdateOrderLineCommand(
    long Id,
    long OrderId,
    long ProductId,
    int Quantity
) : ICommand<Result<OrderLineModel>>;
