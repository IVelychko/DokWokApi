using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Shops;
using Domain.DTOs.Responses.Shops;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetAllShopsByPage;

public sealed class GetAllShopsByPageQueryHandler(IShopService shopService)
    : IQueryHandler<GetAllShopsByPageQuery, IEnumerable<ShopResponse>>
{
    public async Task<IEnumerable<ShopResponse>> Handle(GetAllShopsByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        return await shopService.GetAllAsync(pageInfo);
    }
}
