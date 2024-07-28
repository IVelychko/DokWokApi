using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Shop.Queries.GetAllShops;

public class GetAllShopsQueryHandler(IShopService shopService) : IQueryHandler<GetAllShopsQuery, IEnumerable<ShopModel>>
{
    public async Task<IEnumerable<ShopModel>> Handle(GetAllShopsQuery request, CancellationToken cancellationToken) =>
        await shopService.GetAllAsync();
}
