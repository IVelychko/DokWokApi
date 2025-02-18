using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;

namespace Application.Operations.ProductCategory.Queries.GetProductCategoryById;

public class GetProductCategoryByIdQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetProductCategoryByIdQuery, ProductCategoryResponse?>
{
    public async Task<ProductCategoryResponse?> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        return await productCategoryService.GetByIdAsync(request.Id);
    }
}
