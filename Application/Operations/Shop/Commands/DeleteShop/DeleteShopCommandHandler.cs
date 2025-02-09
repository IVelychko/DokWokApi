using Domain.Abstractions.Messaging;
using Domain.Abstractions.Services;
using Domain.DTOs.Commands.Shops;

namespace Application.Operations.Shop.Commands.DeleteShop;

public class DeleteShopCommandHandler(IShopService shopService) : ICommandHandler<DeleteShopCommand>
{
    public async Task Handle(DeleteShopCommand request, CancellationToken cancellationToken) =>
        await shopService.DeleteAsync(request.Id);
}
