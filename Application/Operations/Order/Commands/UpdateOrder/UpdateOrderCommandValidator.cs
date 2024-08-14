using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.CustomerName).NotEmpty().Matches(RegularExpressions.FirstName).MinimumLength(2);

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber).MinimumLength(9);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.DeliveryAddress).NotEmpty().Matches(RegularExpressions.Address).MinimumLength(5)
            .When(x => x.DeliveryAddress is not null);

        RuleFor(x => x.TotalOrderPrice).NotEmpty();

        RuleFor(x => x.CreationDate).NotEmpty();

        RuleFor(x => x.Status).NotEmpty().MinimumLength(3);

        RuleFor(x => x.PaymentType).NotEmpty().Matches(RegularExpressions.PaymentType).MinimumLength(3);

        RuleFor(x => x.UserId).NotEmpty().Matches(RegularExpressions.Guid).When(x => x.UserId is not null);

        RuleFor(x => x.ShopId).NotEmpty().When(x => x.ShopId is not null);
    }
}
