using FluentValidation;

namespace Application.Operations.ProductCategory.Commands.DeleteProductCategory;

public sealed class DeleteProductCategoryCommandValidator : AbstractValidator<DeleteProductCategoryCommand>
{
    public DeleteProductCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
