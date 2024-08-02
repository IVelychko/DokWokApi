using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.ProductCategory.Commands.AddProductCategory;

public sealed class AddProductCategoryCommandValidator : AbstractValidator<AddProductCategoryCommand>
{
    public AddProductCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Matches(RegularExpressions.RegularString);
    }
}
