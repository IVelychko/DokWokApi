using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.ProductCategory.Queries.GetProductCategoryById;

public class GetProductCategoryByIdQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetProductCategoryByIdQuery, ProductCategoryResponse?>
{
    public async Task<ProductCategoryResponse?> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await productCategoryService.GetByIdAsync(request.Id);
        return category?.ToResponse();
    }
}
