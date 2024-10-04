using Application.Abstractions.Messaging;

namespace Application.Operations.ProductCategory.Commands.DeleteProductCategory;

public sealed record DeleteProductCategoryCommand(long Id) : ICommand;
