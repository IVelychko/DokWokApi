using FluentValidation;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderLineRequestValidator : AbstractValidator<AddTakeawayOrderLineRequest>
{
    public AddTakeawayOrderLineRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
