using Application.Abstractions.Messaging;

namespace Application.Operations.Order.Commands.DeleteOrder;

public sealed record DeleteOrderCommand(long Id) : ICommand<bool?>;
