using Application.Abstractions.Messaging;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryIdAndPage;

public sealed record GetAllProductsByCategoryIdAndPageQuery(long CategoryId, int PageNumber, int PageSize)
    : IQuery<IEnumerable<ProductResponse>>;
