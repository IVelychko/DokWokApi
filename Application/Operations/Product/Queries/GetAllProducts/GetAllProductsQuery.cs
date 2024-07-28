using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProducts;

public sealed record GetAllProductsQuery() : IQuery<IEnumerable<ProductModel>>;
