using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.OrderLine.Commands.AddOrderLine;

public sealed record AddOrderLineCommand(
    long OrderId,
    long ProductId,
    int Quantity
) : ICommand<Result<OrderLineResponse>>;
