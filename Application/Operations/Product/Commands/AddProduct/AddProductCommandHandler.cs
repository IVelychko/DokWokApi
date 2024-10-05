using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.Product.Commands.AddProduct;

public class AddProductCommandHandler(IProductService productService) : ICommandHandler<AddProductCommand, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await productService.AddAsync(model);
        return result.Match(p => p.ToResponse(), Result<ProductResponse>.Failure);
    }
}
