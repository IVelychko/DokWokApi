using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Responses.Products;
using Domain.Models;

namespace Application.Operations.Product.Queries.GetAllProductsByPage;

public sealed class GetAllProductsByPageQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByPageQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByPageQuery request, CancellationToken cancellationToken)
    {
        PageInfo pageInfo = new() { Number = request.PageNumber, Size = request.PageSize };
        var products = await productService.GetAllAsync(pageInfo);
        return products.Select(p => p.ToResponse());
    }
}
