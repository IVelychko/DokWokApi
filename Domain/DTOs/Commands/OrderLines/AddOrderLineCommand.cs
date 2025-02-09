using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.OrderLines;
using Domain.Shared;

namespace Domain.DTOs.Commands.OrderLines;

public sealed record AddOrderLineCommand(
    long OrderId,
    long ProductId,
    int Quantity
) : ICommand<Result<OrderLineResponse>>;
