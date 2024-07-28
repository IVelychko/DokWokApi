using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Shop.Commands.UpdateShop;

public class UpdateShopCommandHandler(IShopService shopService) : ICommandHandler<UpdateShopCommand, Result<ShopModel>>
{
    public async Task<Result<ShopModel>> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await shopService.UpdateAsync(model);
        return result;
    }
}
