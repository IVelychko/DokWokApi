using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Shops;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Application.Operations.Shop.Queries.IsShopAddressTaken;

public class IsShopAddressTakenQueryHandler(IShopService shopService) : IQueryHandler<IsShopAddressTakenQuery, Result<IsTakenResponse>>
{
    public async Task<Result<IsTakenResponse>> Handle(IsShopAddressTakenQuery request, CancellationToken cancellationToken)
    {
        var result = await shopService.IsAddressTakenAsync(request.Street, request.Building);
        Result<IsTakenResponse> isTakenResponseResult = result.Match(isTaken => new IsTakenResponse(isTaken),
            Result<IsTakenResponse>.Failure);

        return isTakenResponseResult;
    }
}
