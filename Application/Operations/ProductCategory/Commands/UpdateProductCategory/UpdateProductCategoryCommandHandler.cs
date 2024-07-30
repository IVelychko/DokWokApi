using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.ResultType;

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
