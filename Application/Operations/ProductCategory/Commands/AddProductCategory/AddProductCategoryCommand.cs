using Application.Abstractions.Messaging;
using Domain.Helpers;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public sealed record AddProductCategoryCommand(string Name) : ICommand<Result<ProductCategoryResponse>>;
