using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Shop.Queries.GetAllShopsByPage;

public sealed class GetAllShopsByPageQueryHandler(IShopService shopService)
    : IQueryHandler<GetAllShopsByPageQuery, IEnumerable<ShopResponse>>
{
    public async Task<IEnumerable<ShopResponse>> Handle(GetAllShopsByPageQuery request, CancellationToken cancellationToken)
    {
        var shops = await shopService.GetAllByPageAsync(request.PageNumber, request.PageSize);
        return shops.Select(s => s.ToResponse());
    }
}
