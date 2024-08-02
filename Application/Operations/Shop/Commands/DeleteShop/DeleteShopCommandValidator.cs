using FluentValidation;

namespace Application.Operations.Shop.Commands.DeleteShop;

public sealed class DeleteShopCommandValidator : AbstractValidator<DeleteShopCommand>
{
    public DeleteShopCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
