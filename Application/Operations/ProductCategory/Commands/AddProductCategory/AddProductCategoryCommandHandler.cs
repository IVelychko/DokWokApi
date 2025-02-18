using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public class AddProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<AddProductCategoryCommand, ProductCategoryResponse>
{
    public async Task<ProductCategoryResponse> Handle(AddProductCategoryCommand request, CancellationToken cancellationToken)
    {
        return await productCategoryService.AddAsync(request);
    }
}
