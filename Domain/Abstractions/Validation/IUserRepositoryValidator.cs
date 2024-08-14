using Domain.Entities;
using FluentValidation.Results;

namespace Domain.Abstractions.Validation;

public interface IUserRepositoryValidator : IBaseValidator<ApplicationUser>
{
    Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword);

    Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(string userId, string newPassword);
}
