using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Products;
using Domain.DTOs.Responses.Products;

namespace Application.Operations.Product.Commands.AddProduct;

public class AddProductCommandHandler(IProductService productService) 
    : ICommandHandler<AddProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        return await productService.AddAsync(request);
    }
}
