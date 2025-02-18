using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Queries.Products;
using Domain.DTOs.Responses;

namespace Application.Operations.Product.Queries.IsProductNameTaken;

public class IsProductNameTakenQueryHandler(IProductService productService)
    : IQueryHandler<IsProductNameTakenQuery, IsTakenResponse>
{
    public async Task<IsTakenResponse> Handle(IsProductNameTakenQuery request, CancellationToken cancellationToken)
    {
        bool isUnique = await productService.IsNameUniqueAsync(request.Name);
        return new IsTakenResponse(!isUnique);
    }
}
