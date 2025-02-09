using Application.Mapping.Extensions;
using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Shops;
using Domain.DTOs.Responses.Shops;
using Domain.Shared;

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
