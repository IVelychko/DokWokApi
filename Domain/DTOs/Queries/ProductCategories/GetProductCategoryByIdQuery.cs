using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.ProductCategories;

namespace Domain.DTOs.Queries.ProductCategories;

public sealed record GetProductCategoryByIdQuery(long Id) : IQuery<ProductCategoryResponse?>;
