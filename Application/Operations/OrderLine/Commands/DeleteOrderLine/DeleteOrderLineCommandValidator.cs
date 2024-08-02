using FluentValidation;

namespace Application.Operations.OrderLine.Commands.DeleteOrderLine;

public sealed class DeleteOrderLineCommandValidator : AbstractValidator<DeleteOrderLineCommand>
{
    public DeleteOrderLineCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
