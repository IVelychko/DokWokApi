using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Shop.Commands.UpdateShop;

public sealed class UpdateShopCommandValidator : AbstractValidator<UpdateShopCommand>
{
    public UpdateShopCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Street).NotEmpty().Matches(RegularExpressions.Street);

        RuleFor(x => x.Building).NotEmpty().Matches(RegularExpressions.Building);

        RuleFor(x => x.OpeningTime).NotEmpty().Matches(RegularExpressions.Hour);

        RuleFor(x => x.ClosingTime).NotEmpty().Matches(RegularExpressions.Hour);
    }
}
