using Application.Abstractions.Messaging;

namespace Application.Operations.Product.Queries.GetAllProductsByPage;

public sealed record GetAllProductsByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<ProductResponse>>;
