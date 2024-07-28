using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductModel>>
{
    public async Task<IEnumerable<ProductModel>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken) =>
        await productService.GetAllAsync();
}
