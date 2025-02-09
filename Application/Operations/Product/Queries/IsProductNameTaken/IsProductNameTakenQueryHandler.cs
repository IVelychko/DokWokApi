using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Responses;
using Domain.Shared;

namespace Application.Operations.Product.Queries.IsProductNameTaken;

public class IsProductNameTakenQueryHandler(IProductService productService)
    : IQueryHandler<IsProductNameTakenQuery, Result<IsTakenResponse>>
{
    public async Task<Result<IsTakenResponse>> Handle(IsProductNameTakenQuery request, CancellationToken cancellationToken)
    {
        var result = await productService.IsNameTakenAsync(request.Name);
        Result<IsTakenResponse> isTakenResponseResult = result.Match(isTaken => new IsTakenResponse(isTaken),
            Result<IsTakenResponse>.Failure);

        return isTakenResponseResult;
    }
}
