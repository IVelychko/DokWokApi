using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Shop.Commands.UpdateShop;

public sealed class UpdateShopCommandValidator : AbstractValidator<UpdateShopCommand>
{
    public UpdateShopCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Street).NotEmpty().Matches(RegularExpressions.Street).MinimumLength(3);

        RuleFor(x => x.Building).NotEmpty().Matches(RegularExpressions.Building).MinimumLength(3);

        RuleFor(x => x.OpeningTime).NotEmpty().Matches(RegularExpressions.Hour).MinimumLength(4);

        RuleFor(x => x.ClosingTime).NotEmpty().Matches(RegularExpressions.Hour).MinimumLength(4);
    }
}
