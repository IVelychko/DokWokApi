using Application.Abstractions.Messaging;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Commands.UpdateProductCategory;

public sealed record UpdateProductCategoryCommand(long Id, string Name) : ICommand<Result<ProductCategoryResponse>>;
