using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProductsByPage;

public sealed class GetAllProductsByPageQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByPageQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var products = await productService.GetAllAsync(pageInfo);
        return products.Select(p => p.ToResponse());
    }
}
