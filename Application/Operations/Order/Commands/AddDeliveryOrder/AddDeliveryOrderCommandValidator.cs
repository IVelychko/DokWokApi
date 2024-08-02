using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.AddDeliveryOrder;

public sealed class AddDeliveryOrderCommandValidator : AbstractValidator<AddDeliveryOrderCommand>
{
    public AddDeliveryOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().Matches(RegularExpressions.FirstName);

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.DeliveryAddress).NotEmpty().Matches(RegularExpressions.Address);

        RuleFor(x => x.PaymentType).NotEmpty().Matches(RegularExpressions.PaymentType);

        RuleFor(x => x.UserId).Matches(RegularExpressions.Guid).When(x => x.UserId is not null);

        RuleForEach(x => x.OrderLines).SetValidator(new AddDeliveryOrderLineRequestValidator())
            .When(x => x.OrderLines is not null && x.OrderLines.Count > 0);
    }
}
