using Domain.Entities;
using Domain.Validation;

namespace Domain.Abstractions.Validation;

public interface IUserRepositoryValidator : IValidator<ApplicationUser>
{
    Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(string? userId, string? oldPassword, string? newPassword);

    Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(string? userId, string? newPassword);

    ValidationResult ValidateCheckPassword(ApplicationUser model, string password);
}
