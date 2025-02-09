using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.OrderLines;

public sealed record DeleteOrderLineCommand(long Id) : ICommand;
