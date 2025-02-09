using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Products;

namespace Domain.DTOs.Queries.Products;

public sealed record GetAllProductsByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<ProductResponse>>;
