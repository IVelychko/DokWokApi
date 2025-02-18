using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Responses.Products;

namespace Application.Operations.Product.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return await productService.GetAllAsync();
    }
}
