using Application.Abstractions.Messaging;

namespace Application.Operations.Product.Commands.DeleteProduct;

public sealed record DeleteProductCommand(long Id) : ICommand<bool?>;
