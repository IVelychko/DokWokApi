using Application.Abstractions.Messaging;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryId;

public sealed record GetAllProductsByCategoryIdQuery(long CategoryId) : IQuery<IEnumerable<ProductResponse>>;
