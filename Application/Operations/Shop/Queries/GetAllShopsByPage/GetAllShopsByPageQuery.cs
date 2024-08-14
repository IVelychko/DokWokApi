using Application.Abstractions.Messaging;

namespace Application.Operations.Shop.Queries.GetAllShopsByPage;

public sealed record GetAllShopsByPageQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<ShopResponse>>;
