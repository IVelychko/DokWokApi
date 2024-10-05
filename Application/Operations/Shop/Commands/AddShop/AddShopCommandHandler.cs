using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.Shop.Commands.AddShop;

public class AddShopCommandHandler(IShopService shopService) : ICommandHandler<AddShopCommand, Result<ShopResponse>>
{
    public async Task<Result<ShopResponse>> Handle(AddShopCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await shopService.AddAsync(model);
        return result.Match(s => s.ToResponse(), Result<ShopResponse>.Failure);
    }
}
