using Application.Constants;
using Domain.Abstractions.Repositories;
using Domain.DTOs.Requests.Shops;
using FluentValidation;

namespace Application.Validators.Shops;

public sealed class DeleteShopRequestValidator : AbstractValidator<DeleteShopRequest>
{
    private readonly IShopRepository _shopRepository;

    public DeleteShopRequestValidator(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(ShopToDeleteExists)
            .WithErrorCode(ErrorCodes.EntityNotFound)
            .WithMessage("There is no shop with this ID to delete in the database");
    }

    private async Task<bool> ShopToDeleteExists(long shopId, CancellationToken cancellationToken)
    {
        return await _shopRepository.ShopExistsAsync(shopId);
    }
}
