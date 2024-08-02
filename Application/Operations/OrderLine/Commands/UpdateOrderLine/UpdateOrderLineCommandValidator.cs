using FluentValidation;

namespace Application.Operations.OrderLine.Commands.UpdateOrderLine;

public sealed class UpdateOrderLineCommandValidator : AbstractValidator<UpdateOrderLineCommand>
{
    public UpdateOrderLineCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.OrderId).NotEmpty();

        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
