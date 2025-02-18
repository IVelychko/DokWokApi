using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Shops;
using Domain.DTOs.Responses.Shops;

namespace Application.Operations.Shop.Queries.GetShopById;

public class GetShopByIdQueryHandler(IShopService shopService) : IQueryHandler<GetShopByIdQuery, ShopResponse?>
{
    public async Task<ShopResponse?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        return await shopService.GetByIdAsync(request.Id);
    }
}
