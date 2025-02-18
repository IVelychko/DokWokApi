using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Responses.Products;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryId;

public class GetAllProductsByCategoryIdQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByCategoryIdQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        return await productService.GetAllByCategoryIdAsync(request.CategoryId);
    }
}
