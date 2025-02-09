using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Orders;

public sealed record DeleteOrderCommand(long Id) : ICommand;
