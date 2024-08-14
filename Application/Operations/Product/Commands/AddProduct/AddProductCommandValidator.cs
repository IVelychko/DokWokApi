using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Product.Commands.AddProduct;

public sealed class AddProductCommandValidator : AbstractValidator<AddProductCommand>
{
    public AddProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Matches(RegularExpressions.RegularString).MinimumLength(3);

        RuleFor(x => x.Price).GreaterThan(0);

        RuleFor(x => x.Weight).GreaterThan(0);

        RuleFor(x => x.MeasurementUnit).NotEmpty().Matches(RegularExpressions.RegularString);

        RuleFor(x => x.Description).NotEmpty().Matches(RegularExpressions.RegularString).MinimumLength(5);

        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
