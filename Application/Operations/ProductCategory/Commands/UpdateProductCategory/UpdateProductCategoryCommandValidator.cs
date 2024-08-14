using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.ProductCategory.Commands.UpdateProductCategory;

public sealed class UpdateProductCategoryCommandValidator : AbstractValidator<UpdateProductCategoryCommand>
{
    public UpdateProductCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().Matches(RegularExpressions.RegularString).MinimumLength(3);
    }
}
