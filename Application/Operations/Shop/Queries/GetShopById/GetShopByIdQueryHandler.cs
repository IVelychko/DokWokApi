using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Shop.Queries.GetShopById;

public class GetShopByIdQueryHandler(IShopService shopService) : IQueryHandler<GetShopByIdQuery, ShopResponse?>
{
    public async Task<ShopResponse?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        var shop = await shopService.GetByIdAsync(request.Id);
        return shop?.ToResponse();
    }
}
