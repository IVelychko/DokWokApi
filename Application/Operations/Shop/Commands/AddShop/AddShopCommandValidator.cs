using Domain.Abstractions.Repositories;
using Domain.DTOs.Commands.Shops;
using Domain.Shared;
using FluentValidation;

namespace Application.Operations.Shop.Commands.AddShop;

public sealed class AddShopCommandValidator : AbstractValidator<AddShopCommand>
{
    private readonly IShopRepository _shopRepository;

    public AddShopCommandValidator(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;

        RuleLevelCascadeMode = CascadeMode.Stop;

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
                            .WithName("shop")
                            .WithMessage("The shop with the same Street and Building values is already present in the database");
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

    private async Task<bool> IsAddressTaken(AddShopCommand shop, CancellationToken cancellationToken)
    {
        var result = await _shopRepository.IsAddressTakenAsync(shop.Street, shop.Building);
        return result.Match(isTaken => !isTaken, error => false);
    }
}
