using Application.Abstractions.Messaging;

namespace Application.Operations.Product.Queries.GetProductById;

public sealed record GetProductByIdQuery(long Id) : IQuery<ProductResponse?>;
