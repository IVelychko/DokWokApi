using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.ProductCategories;
using Domain.DTOs.Responses;

namespace Application.Operations.ProductCategory.Queries.IsProductCategoryNameTaken;

public class IsProductCategoryNameTakenQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<IsProductCategoryNameTakenQuery, IsTakenResponse>
{
    public async Task<IsTakenResponse> Handle(IsProductCategoryNameTakenQuery request, CancellationToken cancellationToken)
    {
        bool isUnique = await productCategoryService.IsNameUniqueAsync(request.Name);
        return new IsTakenResponse(!isUnique);
    }
}
