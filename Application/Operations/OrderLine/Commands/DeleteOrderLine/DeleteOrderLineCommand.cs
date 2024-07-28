using Application.Abstractions.Messaging;

namespace Application.Operations.OrderLine.Commands.DeleteOrderLine;

public sealed record DeleteOrderLineCommand(long Id) : ICommand<bool?>;
