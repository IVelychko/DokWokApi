using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategoriesByPage;

public sealed class GetAllProductCategoriesByPageQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetAllProductCategoriesByPageQuery, IEnumerable<ProductCategoryResponse>>
{
    public async Task<IEnumerable<ProductCategoryResponse>> Handle(GetAllProductCategoriesByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var categories = await productCategoryService.GetAllAsync(pageInfo);
        return categories.Select(c => c.ToResponse());
    }
}
