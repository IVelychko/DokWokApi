using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Models;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategoriesByPage;

public sealed class GetAllProductCategoriesByPageQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetAllProductCategoriesByPageQuery, IEnumerable<ProductCategoryResponse>>
{
    public async Task<IEnumerable<ProductCategoryResponse>> Handle(GetAllProductCategoriesByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        return await productCategoryService.GetAllAsync(pageInfo);
    }
}
