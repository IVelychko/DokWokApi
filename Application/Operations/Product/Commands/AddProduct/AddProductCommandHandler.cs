using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Products;
using Domain.DTOs.Responses.Products;
using Domain.Shared;

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
