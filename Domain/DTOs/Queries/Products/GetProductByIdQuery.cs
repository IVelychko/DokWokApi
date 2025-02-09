using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Products;

namespace Domain.DTOs.Queries.Products;

public sealed record GetProductByIdQuery(long Id) : IQuery<ProductResponse?>;
