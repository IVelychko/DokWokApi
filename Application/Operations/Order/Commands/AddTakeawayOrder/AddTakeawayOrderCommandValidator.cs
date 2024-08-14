using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderCommandValidator : AbstractValidator<AddTakeawayOrderCommand>
{
    public AddTakeawayOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().Matches(RegularExpressions.FirstName).MinimumLength(2);

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber).MinimumLength(9);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.ShopId).NotEmpty();

        RuleFor(x => x.PaymentType).NotEmpty().Matches(RegularExpressions.PaymentType).MinimumLength(5);

        RuleFor(x => x.UserId).NotEmpty().Matches(RegularExpressions.Guid).When(x => x.UserId is not null);

        RuleForEach(x => x.OrderLines).SetValidator(new AddTakeawayOrderLineRequestValidator())
            .When(x => x.OrderLines is not null && x.OrderLines.Count > 0);
    }
}
