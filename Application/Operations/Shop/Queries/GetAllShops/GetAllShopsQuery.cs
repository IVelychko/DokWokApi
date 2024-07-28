using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetAllShops;

public sealed record GetAllShopsQuery() : IQuery<IEnumerable<ShopModel>>;
