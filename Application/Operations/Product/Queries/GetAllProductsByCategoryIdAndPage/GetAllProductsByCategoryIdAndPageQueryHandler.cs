using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryIdAndPage;

public sealed class GetAllProductsByCategoryIdAndPageQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByCategoryIdAndPageQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByCategoryIdAndPageQuery request, CancellationToken cancellationToken)
    {
        var products = await productService.GetAllByCategoryIdAndPageAsync(request.CategoryId, request.PageNumber, request.PageSize);
        return products.Select(p => p.ToResponse());
    }
}
