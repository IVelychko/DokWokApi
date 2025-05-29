using Domain.Abstractions.Repositories;
using Domain.Constants;
using Domain.DTOs.Requests.ProductCategories;
using FluentValidation;

namespace Application.Validators.ProductCategories;

public sealed class AddProductCategoryRequestValidator : AbstractValidator<AddProductCategoryRequest>
{
    private readonly IProductCategoryRepository _productCategoryRepository;

    public AddProductCategoryRequestValidator(IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository = productCategoryRepository;

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Matches(RegularExpressions.RegularString)
            .MinimumLength(3)
            .MustAsync(IsNameUnique)
            .WithMessage("The product category with the same Name value is already present in the database");
    }
    
    private async Task<bool> IsNameUnique(string name, CancellationToken token)
    {
        return await _productCategoryRepository.IsNameUniqueAsync(name);
    }
}
