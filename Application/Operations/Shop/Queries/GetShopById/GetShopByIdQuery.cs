using Application.Abstractions.Messaging;

namespace Application.Operations.Shop.Queries.GetShopById;

public sealed record GetShopByIdQuery(long Id) : IQuery<ShopResponse?>;
