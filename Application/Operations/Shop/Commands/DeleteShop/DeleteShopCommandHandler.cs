using Application.Abstractions.Messaging;
using Domain.Abstractions.Services;

namespace Application.Operations.Shop.Commands.DeleteShop;

public class DeleteShopCommandHandler(IShopService shopService) : ICommandHandler<DeleteShopCommand, bool?>
{
    public async Task<bool?> Handle(DeleteShopCommand request, CancellationToken cancellationToken) =>
        await shopService.DeleteAsync(request.Id);
}
