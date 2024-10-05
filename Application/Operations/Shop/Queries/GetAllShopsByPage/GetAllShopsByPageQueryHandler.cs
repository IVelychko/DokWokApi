using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetAllShopsByPage;

public sealed class GetAllShopsByPageQueryHandler(IShopService shopService)
    : IQueryHandler<GetAllShopsByPageQuery, IEnumerable<ShopResponse>>
{
    public async Task<IEnumerable<ShopResponse>> Handle(GetAllShopsByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var shops = await shopService.GetAllAsync(pageInfo);
        return shops.Select(s => s.ToResponse());
    }
}
