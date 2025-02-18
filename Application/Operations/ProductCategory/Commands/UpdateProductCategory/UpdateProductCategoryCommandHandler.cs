using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;

namespace Application.Operations.ProductCategory.Commands.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<UpdateProductCategoryCommand, ProductCategoryResponse>
{
    public async Task<ProductCategoryResponse> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        return await productCategoryService.UpdateAsync(request);
    }
}
