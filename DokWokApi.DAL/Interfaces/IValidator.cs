using DokWokApi.DAL.Validation;

namespace DokWokApi.DAL.Interfaces;

public interface IValidator<TModel> where TModel : class
{
    Task<ValidationResult> ValidateAddAsync(TModel? model);

    Task<ValidationResult> ValidateUpdateAsync(TModel? model);
}