using Domain.Helpers;
using FluentValidation;

namespace Application.Operations.Product.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().Matches(RegularExpressions.RegularString).MinimumLength(3);

        RuleFor(x => x.Price).GreaterThan(0);

        RuleFor(x => x.Weight).GreaterThan(0);

        RuleFor(x => x.MeasurementUnit).NotEmpty().Matches(RegularExpressions.RegularString);

        RuleFor(x => x.Description).NotEmpty().Matches(RegularExpressions.RegularString).MinimumLength(5);

        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
