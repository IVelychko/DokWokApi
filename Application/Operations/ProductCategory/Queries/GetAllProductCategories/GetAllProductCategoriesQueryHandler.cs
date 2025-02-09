using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;

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
