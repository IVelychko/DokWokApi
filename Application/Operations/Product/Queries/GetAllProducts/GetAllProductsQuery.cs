using Application.Abstractions.Messaging;

namespace Application.Operations.Product.Queries.GetAllProducts;

public sealed record GetAllProductsQuery() : IQuery<IEnumerable<ProductResponse>>;
