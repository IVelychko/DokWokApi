using Application.Abstractions.Messaging;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public sealed record AddOrderLineCommand(
    long OrderId,
    long ProductId,
    int Quantity
) : ICommand<Result<OrderLineModel>>;
