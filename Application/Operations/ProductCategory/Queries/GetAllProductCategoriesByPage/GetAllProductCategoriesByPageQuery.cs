using Application.Abstractions.Messaging;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategoriesByPage;

public sealed record GetAllProductCategoriesByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<ProductCategoryResponse>>;
