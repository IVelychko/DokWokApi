using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.ResultType;

namespace Application.Operations.Product.Queries.IsProductNameTaken;

public class IsProductNameTakenQueryHandler(IProductService productService)
    : IQueryHandler<IsProductNameTakenQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(IsProductNameTakenQuery request, CancellationToken cancellationToken) =>
        await productService.IsNameTakenAsync(request.Name);
}
