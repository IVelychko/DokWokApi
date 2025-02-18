using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Shops;
using Domain.DTOs.Responses;

namespace Application.Operations.Shop.Queries.IsShopAddressTaken;

public class IsShopAddressTakenQueryHandler(IShopService shopService) : IQueryHandler<IsShopAddressTakenQuery, IsTakenResponse>
{
    public async Task<IsTakenResponse> Handle(IsShopAddressTakenQuery request, CancellationToken cancellationToken)
    {
        var isUnique = await shopService.IsAddressUniqueAsync(request.Street, request.Building);
        return new IsTakenResponse(!isUnique);
    }
}
