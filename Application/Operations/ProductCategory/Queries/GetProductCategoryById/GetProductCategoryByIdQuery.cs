using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.ProductCategory.Queries.GetProductCategoryById;

public sealed record GetProductCategoryByIdQuery(long Id) : IQuery<ProductCategoryModel?>;
