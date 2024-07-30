﻿using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;

namespace Application.Operations.Product.Queries.GetAllProductsByCategoryId;

public class GetAllProductsByCategoryIdQueryHandler(IProductService productService)
    : IQueryHandler<GetAllProductsByCategoryIdQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsByCategoryIdQuery request, CancellationToken cancellationToken)
    {
        var products = await productService.GetAllByCategoryIdAsync(request.CategoryId);
        return products.Select(p => p.ToResponse());
    }
}
