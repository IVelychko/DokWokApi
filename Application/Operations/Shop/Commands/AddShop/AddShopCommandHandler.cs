using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Shops;
using Domain.DTOs.Responses.Shops;

namespace Application.Operations.Shop.Commands.AddShop;

public class AddShopCommandHandler(IShopService shopService) : ICommandHandler<AddShopCommand, ShopResponse>
{
    public async Task<ShopResponse> Handle(AddShopCommand request, CancellationToken cancellationToken)
    {
        return await shopService.AddAsync(request);
    }
}
