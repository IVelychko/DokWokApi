using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.Shop.Queries.IsShopAddressTaken;

public class IsShopAddressTakenQueryHandler(IShopService shopService) : IQueryHandler<IsShopAddressTakenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsShopAddressTakenQuery request, CancellationToken cancellationToken) =>
        await shopService.IsAddressTakenAsync(request.Street, request.Building);
}
