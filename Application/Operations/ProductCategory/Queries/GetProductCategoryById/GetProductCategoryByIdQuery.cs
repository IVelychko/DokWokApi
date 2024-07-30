using Application.Abstractions.Messaging;

namespace Application.Operations.ProductCategory.Queries.GetProductCategoryById;

public sealed record GetProductCategoryByIdQuery(long Id) : IQuery<ProductCategoryResponse?>;
