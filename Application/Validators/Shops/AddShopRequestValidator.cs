using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.Shops;
using FluentValidation;

namespace Application.Validators.Shops;

public sealed class AddShopRequestValidator : AbstractValidator<AddShopRequest>
{
    private readonly IShopRepository _shopRepository;

    public AddShopRequestValidator(IShopRepository shopRepository)
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
                            .MustAsync(IsAddressUnique)
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

    private async Task<bool> IsAddressUnique(AddShopRequest shop, CancellationToken cancellationToken)
    {
        return await _shopRepository.IsAddressUniqueAsync(shop.Street, shop.Building);
    }
}
