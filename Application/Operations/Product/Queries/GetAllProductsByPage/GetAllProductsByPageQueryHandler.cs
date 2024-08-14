using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Product.Queries.GetAllProductsByPage;

public sealed class GetAllProductsByPageQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByPageQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByPageQuery request, CancellationToken cancellationToken)
    {
        var products = await productService.GetAllByPageAsync(request.PageNumber, request.PageSize);
        return products.Select(p => p.ToResponse());
    }
}
