using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Product.Commands.AddProduct;

public class AddProductCommandHandler(IProductService productService) : ICommandHandler<AddProductCommand, Result<ProductModel>>
{
    public async Task<Result<ProductModel>> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productService.AddAsync(model);
        return result;
    }
}
