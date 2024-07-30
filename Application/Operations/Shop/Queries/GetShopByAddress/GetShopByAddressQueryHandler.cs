using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Shop.Queries.GetShopByAddress;

public class GetShopByAddressQueryHandler(IShopService shopService) : IQueryHandler<GetShopByAddressQuery, ShopResponse?>
{
    public async Task<ShopResponse?> Handle(GetShopByAddressQuery request, CancellationToken cancellationToken)
    {
        var shop = await shopService.GetByAddressAsync(request.Street, request.Building);
        return shop?.ToResponse();
    }
}
