using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Products;

namespace Domain.DTOs.Queries.Products;

public sealed record GetAllProductsByCategoryIdQuery(long CategoryId) : IQuery<IEnumerable<ProductResponse>>;
