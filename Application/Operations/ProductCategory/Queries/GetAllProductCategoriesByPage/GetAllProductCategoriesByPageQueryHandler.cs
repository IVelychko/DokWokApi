using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.ProductCategory.Queries.GetAllProductCategoriesByPage;

public sealed class GetAllProductCategoriesByPageQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetAllProductCategoriesByPageQuery, IEnumerable<ProductCategoryResponse>>
{
    public async Task<IEnumerable<ProductCategoryResponse>> Handle(GetAllProductCategoriesByPageQuery request, CancellationToken cancellationToken)
    {
        var categories = await productCategoryService.GetAllByPageAsync(request.PageNumber, request.PageSize);
        return categories.Select(c => c.ToResponse());
    }
}
