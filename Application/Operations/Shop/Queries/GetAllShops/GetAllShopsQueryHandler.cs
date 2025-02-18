using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Shops;
using Domain.DTOs.Responses.Shops;

namespace Application.Operations.Shop.Queries.GetAllShops;

public class GetAllShopsQueryHandler(IShopService shopService) : IQueryHandler<GetAllShopsQuery, IEnumerable<ShopResponse>>
{
    public async Task<IEnumerable<ShopResponse>> Handle(GetAllShopsQuery request, CancellationToken cancellationToken)
    {
        return await shopService.GetAllAsync();
    }
}
