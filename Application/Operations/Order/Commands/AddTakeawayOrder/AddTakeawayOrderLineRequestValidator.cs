using FluentValidation;

namespace Application.Operations.Order.Commands.AddTakeawayOrder;

public class AddTakeawayOrderLineRequestValidator : AbstractValidator<AddTakeawayOrderLineRequest>
{
    public AddTakeawayOrderLineRequestValidator()
    {
        RuleFor(x => x.ProductId).NotNull();

        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
