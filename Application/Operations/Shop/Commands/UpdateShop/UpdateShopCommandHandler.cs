using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Helpers;

namespace Application.Operations.Shop.Commands.UpdateShop;

public class UpdateShopCommandHandler(IShopService shopService) : ICommandHandler<UpdateShopCommand, Result<ShopResponse>>
{
    public async Task<Result<ShopResponse>> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await shopService.UpdateAsync(model);
        return result.Match(s => s.ToResponse(), Result<ShopResponse>.Failure);
    }
}
