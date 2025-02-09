using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Shops;

namespace Domain.DTOs.Queries.Shops;

public sealed record GetShopByIdQuery(long Id) : IQuery<ShopResponse?>;
