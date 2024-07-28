using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductService productService) : ICommandHandler<UpdateProductCommand, Result<ProductModel>>
{
    public async Task<Result<ProductModel>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productService.UpdateAsync(model);
        return result;
    }
}
