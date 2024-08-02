using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Order.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.CustomerName).NotEmpty().Matches(RegularExpressions.FirstName);

        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(RegularExpressions.PhoneNumber);

        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.DeliveryAddress).Matches(RegularExpressions.Address).When(x => x.DeliveryAddress is not null);

        RuleFor(x => x.TotalOrderPrice).NotNull();

        RuleFor(x => x.CreationDate).NotNull();

        RuleFor(x => x.Status).NotEmpty();

        RuleFor(x => x.PaymentType).NotEmpty().Matches(RegularExpressions.PaymentType);

        RuleFor(x => x.UserId).Matches(RegularExpressions.Guid).When(x => x.UserId is not null);

        RuleFor(x => x.ShopId).NotEmpty().When(x => x.ShopId is not null);
    }
}
