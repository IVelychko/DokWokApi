using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.Operations.Shop.Commands.DeleteShop;

public sealed class DeleteShopCommandValidator : AbstractValidator<DeleteShopCommand>
{
    private readonly IShopRepository _shopRepository;

    public DeleteShopCommandValidator(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ShopToDeleteExists)
            .WithErrorCode("404")
            .WithMessage("There is no shop with this ID to delete in the database");
    }

    private async Task<bool> ShopToDeleteExists(long shopId, CancellationToken cancellationToken) =>
        (await _shopRepository.GetByIdAsync(shopId)) is not null;
}
