using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;

public class IsProductCategoryNameTakenQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<IsProductCategoryNameTakenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsProductCategoryNameTakenQuery request, CancellationToken cancellationToken) =>
        await productCategoryService.IsNameTakenAsync(request.Name);
}
