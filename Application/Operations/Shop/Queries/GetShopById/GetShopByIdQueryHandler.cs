using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetShopById;

public class GetShopByIdQueryHandler(IShopService shopService) : IQueryHandler<GetShopByIdQuery, ShopModel?>
{
    public async Task<ShopModel?> Handle(GetShopByIdQuery request, CancellationToken cancellationToken) =>
        await shopService.GetByIdAsync(request.Id);
}
