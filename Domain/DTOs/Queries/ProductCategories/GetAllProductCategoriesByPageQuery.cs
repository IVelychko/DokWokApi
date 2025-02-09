using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.ProductCategories;

namespace Domain.DTOs.Queries.ProductCategories;

public sealed record GetAllProductCategoriesByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<ProductCategoryResponse>>;
