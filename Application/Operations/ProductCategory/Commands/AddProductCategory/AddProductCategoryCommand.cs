using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public sealed record AddProductCategoryCommand(string Name) : ICommand<Result<ProductCategoryResponse>>;
