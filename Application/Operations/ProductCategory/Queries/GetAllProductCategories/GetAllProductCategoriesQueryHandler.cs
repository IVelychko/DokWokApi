using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategories;

public class GetAllProductCategoriesQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetAllProductCategoriesQuery, IEnumerable<ProductCategoryModel>>
{
    public async Task<IEnumerable<ProductCategoryModel>> Handle(GetAllProductCategoriesQuery request, CancellationToken cancellationToken) =>
        await productCategoryService.GetAllAsync();
}
