using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Product.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productService.GetAllAsync();
        return products.Select(p => p.ToResponse());
    }
}
