using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryIdAndPage;

public sealed class GetAllProductsByCategoryIdAndPageQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByCategoryIdAndPageQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByCategoryIdAndPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var products = await productService.GetAllByCategoryIdAsync(request.CategoryId, pageInfo);
        return products.Select(p => p.ToResponse());
    }
}
