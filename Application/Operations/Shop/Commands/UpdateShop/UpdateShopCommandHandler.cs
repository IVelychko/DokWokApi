using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Shops;
using Domain.DTOs.Responses.Shops;

namespace Application.Operations.Shop.Commands.UpdateShop;

public class UpdateShopCommandHandler(IShopService shopService) : ICommandHandler<UpdateShopCommand, ShopResponse>
{
    public async Task<ShopResponse> Handle(UpdateShopCommand request, CancellationToken cancellationToken)
    {
        return await shopService.UpdateAsync(request);
    }
}
