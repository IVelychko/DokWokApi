﻿using Domain.Validation;

namespace Domain.Abstractions.Validation;

public interface IValidator<TModel> where TModel : class
{
    Task<ValidationResult> ValidateAddAsync(TModel? model);

    Task<ValidationResult> ValidateUpdateAsync(TModel? model);
}