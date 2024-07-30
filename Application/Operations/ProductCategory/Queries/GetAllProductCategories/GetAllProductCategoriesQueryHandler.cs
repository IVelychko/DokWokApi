using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategories;

public class GetAllProductCategoriesQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetAllProductCategoriesQuery, IEnumerable<ProductCategoryResponse>>
{
    public async Task<IEnumerable<ProductCategoryResponse>> Handle(GetAllProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await productCategoryService.GetAllAsync();
        return categories.Select(c => c.ToResponse());
    }
}
