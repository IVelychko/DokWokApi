using Domain.Validation;

namespace Domain.Abstractions.Validation;

public interface IValidator<TModel>
{
    Task<ValidationResult> ValidateAddAsync(TModel? model);

    Task<ValidationResult> ValidateUpdateAsync(TModel? model);
}