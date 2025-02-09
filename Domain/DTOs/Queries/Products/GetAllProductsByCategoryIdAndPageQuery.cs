using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Products;

namespace Domain.DTOs.Queries.Products;

public sealed record GetAllProductsByCategoryIdAndPageQuery(long CategoryId, int PageNumber, int PageSize)
    : IQuery<IEnumerable<ProductResponse>>;
