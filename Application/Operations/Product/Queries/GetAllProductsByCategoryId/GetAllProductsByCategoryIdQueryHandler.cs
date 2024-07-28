using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryId;

public class GetAllProductsByCategoryIdQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByCategoryIdQuery, IEnumerable<ProductModel>>
{
    public async Task<IEnumerable<ProductModel>> Handle(GetAllProductsByCategoryIdQuery request, CancellationToken cancellationToken) =>
        await productService.GetAllByCategoryIdAsync(request.CategoryId);
}
