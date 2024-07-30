using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public class AddProductCategoryCommandHandler(IProductCategoryService productCategoryService)
    : ICommandHandler<AddProductCategoryCommand, Result<ProductCategoryResponse>>
{
    public async Task<Result<ProductCategoryResponse>> Handle(AddProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productCategoryService.AddAsync(model);
        return result.Match(c => c.ToResponse(), Result<ProductCategoryResponse>.Failure);
    }
}
