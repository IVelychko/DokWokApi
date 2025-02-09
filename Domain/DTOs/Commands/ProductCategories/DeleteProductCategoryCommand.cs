using Domain.Abstractions.Messaging;

namespace Domain.DTOs.Commands.ProductCategories;

public sealed record DeleteProductCategoryCommand(long Id) : ICommand;
