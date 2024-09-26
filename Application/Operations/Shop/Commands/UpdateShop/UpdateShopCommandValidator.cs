using Domain.Abstractions.Repositories;
using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Shop.Commands.UpdateShop;

public sealed class UpdateShopCommandValidator : AbstractValidator<UpdateShopCommand>
{
    private readonly IShopRepository _shopRepository;

    public UpdateShopCommandValidator(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(ShopToUpdateExists)
            .WithErrorCode("404")
            .WithMessage("There is no entity with this ID in the database");

        RuleFor(x => x.Street)
            .NotEmpty()
            .Matches(RegularExpressions.Street)
            .MinimumLength(3)
            .DependentRules(() =>
            {
                RuleFor(x => x.Building)
                    .NotEmpty()
                    .Matches(RegularExpressions.Building)
                    .MaximumLength(6)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x)
                            .MustAsync(IsAddressTaken)
                            .WithName("address")
                            .WithMessage("The shop with the same Street and Building values is already present in the database")
                            .When(x => x.Id != 0);
                    });
            });

        RuleFor(x => x.OpeningTime)
            .NotEmpty()
            .Matches(RegularExpressions.Hour)
            .MinimumLength(4);

        RuleFor(x => x.ClosingTime)
            .NotEmpty()
            .Matches(RegularExpressions.Hour)
            .MinimumLength(4);
    }

    private async Task<bool> ShopToUpdateExists(long shopId, CancellationToken cancellationToken)
    {
        return (await _shopRepository.GetByIdAsync(shopId)) is not null;
    }

    private async Task<bool> IsAddressTaken(UpdateShopCommand shop, CancellationToken cancellationToken)
    {
        var existsingEntity = await _shopRepository.GetByIdAsync(shop.Id);
        if (existsingEntity is null)
        {
            return false;
        }

        if (shop.Street != existsingEntity.Street || shop.Building != existsingEntity.Building)
        {
            var result = await _shopRepository.IsAddressTakenAsync(shop.Street, shop.Building);
            return result.Match(isTaken => !isTaken, error => false);
        }

        return true;
    }
}
