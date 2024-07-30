using Application.Abstractions.Messaging;

namespace Application.Operations.Shop.Queries.GetAllShops;

public sealed record GetAllShopsQuery() : IQuery<IEnumerable<ShopResponse>>;
