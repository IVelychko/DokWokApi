using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;

public class IsProductCategoryNameTakenQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<IsProductCategoryNameTakenQuery, Result<IsTakenResponse>>
{
    public async Task<Result<IsTakenResponse>> Handle(IsProductCategoryNameTakenQuery request, CancellationToken cancellationToken)
    {
        var result = await productCategoryService.IsNameTakenAsync(request.Name);
        Result<IsTakenResponse> isTakenResponseResult = result.Match(isTaken => new IsTakenResponse(isTaken),
            Result<IsTakenResponse>.Failure);

        return isTakenResponseResult;
    }
}
