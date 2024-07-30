using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductService productService) : ICommandHandler<UpdateProductCommand, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productService.UpdateAsync(model);
        return result.Match(p => p.ToResponse(), Result<ProductResponse>.Failure);
    }
}
