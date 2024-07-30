using Application.Abstractions.Messaging;

namespace Application.Operations.Shop.Queries.GetShopByAddress;

public sealed record GetShopByAddressQuery(string Street, string Building) : IQuery<ShopResponse?>;
