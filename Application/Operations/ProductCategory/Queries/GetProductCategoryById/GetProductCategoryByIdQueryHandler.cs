using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.ProductCategory.Queries.GetProductCategoryById;

public class GetProductCategoryByIdQueryHandler(IProductCategoryService productCategoryService)
    : IQueryHandler<GetProductCategoryByIdQuery, ProductCategoryModel?>
{
    public async Task<ProductCategoryModel?> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken) =>
        await productCategoryService.GetByIdAsync(request.Id);
}
