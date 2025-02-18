using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Responses.Products;

namespace Application.Operations.Product.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductService productService) : IQueryHandler<GetProductByIdQuery, ProductResponse?>
{
    public async Task<ProductResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await productService.GetByIdAsync(request.Id);
    }
}
