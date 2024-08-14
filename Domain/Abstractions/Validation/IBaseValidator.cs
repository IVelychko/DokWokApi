using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IBaseValidator<TModel>
{
    Task<ValidationResult> ValidateAddAsync(TModel model);

    Task<ValidationResult> ValidateUpdateAsync(TModel model);
}
