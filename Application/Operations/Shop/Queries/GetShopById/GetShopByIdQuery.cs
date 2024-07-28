using Application.Abstractions.Messaging;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetShopById;

public sealed record GetShopByIdQuery(long Id) : IQuery<ShopModel?>;
