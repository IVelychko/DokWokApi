using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetShopByAddress;

public sealed record GetShopByAddressQuery(string Street, string Building) : IQuery<ShopModel?>;
