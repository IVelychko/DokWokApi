using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryId;

public sealed record GetAllProductsByCategoryIdQuery(long CategoryId) : IQuery<IEnumerable<ProductModel>>;
