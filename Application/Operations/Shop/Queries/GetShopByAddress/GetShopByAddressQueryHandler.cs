using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetShopByAddress;

public class GetShopByAddressQueryHandler(IShopService shopService) : IQueryHandler<GetShopByAddressQuery, ShopModel?>
{
    public async Task<ShopModel?> Handle(GetShopByAddressQuery request, CancellationToken cancellationToken) =>
        await shopService.GetByAddressAsync(request.Street, request.Building);
}
