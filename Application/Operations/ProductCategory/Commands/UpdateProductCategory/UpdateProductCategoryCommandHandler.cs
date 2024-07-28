using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Commands.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<UpdateProductCategoryCommand, Result<ProductCategoryModel>>
{
    public async Task<Result<ProductCategoryModel>> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productCategoryService.UpdateAsync(model);
        return result;
    }
}
