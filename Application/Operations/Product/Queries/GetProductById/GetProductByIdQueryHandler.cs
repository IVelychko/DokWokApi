using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Product.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductService productService) : IQueryHandler<GetProductByIdQuery, ProductResponse?>
{
    public async Task<ProductResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(request.Id);
        return product?.ToResponse();
    }
}
