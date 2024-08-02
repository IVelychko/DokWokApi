using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Shop.Commands.AddShop;

public sealed class AddShopCommandValidator : AbstractValidator<AddShopCommand>
{
    public AddShopCommandValidator()
    {
        RuleFor(x => x.Street).NotEmpty().Matches(RegularExpressions.Street);

        RuleFor(x => x.Building).NotEmpty().Matches(RegularExpressions.Building);

        RuleFor(x => x.OpeningTime).NotEmpty().Matches(RegularExpressions.Hour);

        RuleFor(x => x.ClosingTime).NotEmpty().Matches(RegularExpressions.Hour);
    }
}
