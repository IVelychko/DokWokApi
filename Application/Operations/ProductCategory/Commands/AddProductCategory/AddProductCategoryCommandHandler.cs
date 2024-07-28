using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public class AddProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<AddProductCategoryCommand, Result<ProductCategoryModel>>
{
    public async Task<Result<ProductCategoryModel>> Handle(AddProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productCategoryService.AddAsync(model);
        return result;
    }
}
