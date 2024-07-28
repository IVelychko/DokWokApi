using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetProductById;

public sealed record GetProductByIdQuery(long Id) : IQuery<ProductModel?>;
