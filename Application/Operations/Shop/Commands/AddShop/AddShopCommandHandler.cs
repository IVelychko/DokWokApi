using Application.Abstractions.Messaging;
using Application.Mapping.Extensions;
using Domain.Abstractions.Services;
using Domain.Models;
using Domain.ResultType;

namespace Application.Operations.Shop.Commands.AddShop;

public class AddShopCommandHandler(IShopService shopService) : ICommandHandler<AddShopCommand, Result<ShopModel>>
{
    public async Task<Result<ShopModel>> Handle(AddShopCommand request, CancellationToken cancellationToken)
    {
        var model = request.ToModel();
        var result = await shopService.AddAsync(model);
        return result;
    }
}
