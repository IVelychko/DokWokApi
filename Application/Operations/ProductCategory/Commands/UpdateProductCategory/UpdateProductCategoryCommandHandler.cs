using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.ProductCategories;
using Domain.DTOs.Responses.ProductCategories;
using Domain.Shared;

namespace Application.Operations.ProductCategory.Commands.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<UpdateProductCategoryCommand, Result<ProductCategoryResponse>>
{
    public async Task<Result<ProductCategoryResponse>> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productCategoryService.UpdateAsync(model);
        return result.Match(c => c.ToResponse(), Result<ProductCategoryResponse>.Failure);
    }
}
