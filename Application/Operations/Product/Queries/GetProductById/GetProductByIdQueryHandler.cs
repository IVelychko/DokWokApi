using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetProductById;

public class GetProductByIdQueryHandler(IProductService productService) : IQueryHandler<GetProductByIdQuery, ProductModel?>
{
    public async Task<ProductModel?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) =>
        await productService.GetByIdAsync(request.Id);
}
