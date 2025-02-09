using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.Products;

public sealed record DeleteProductCommand(long Id) : ICommand;
