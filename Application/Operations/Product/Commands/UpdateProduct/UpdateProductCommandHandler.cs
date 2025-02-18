using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Products;
using Domain.DTOs.Responses.Products;

namespace Application.Operations.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductService productService) 
    : ICommandHandler<UpdateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await productService.UpdateAsync(request);
    }
}
