using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Shops;

namespace Domain.DTOs.Queries.Shops;

public sealed record GetShopByAddressQuery(string Street, string Building) : IQuery<ShopResponse?>;
