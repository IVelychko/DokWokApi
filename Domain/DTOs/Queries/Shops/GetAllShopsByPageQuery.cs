using Domain.Abstractions.Messaging;
using Domain.DTOs.Responses.Shops;

namespace Domain.DTOs.Queries.Shops;

public sealed record GetAllShopsByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<ShopResponse>>;
