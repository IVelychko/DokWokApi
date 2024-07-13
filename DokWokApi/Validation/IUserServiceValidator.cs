using DokWokApi.BLL.Models.User;

namespace DokWokApi.Validation;

public interface IUserServiceValidator : IValidator<UserModel>
{
    Task<ValidationResult> ValidateUpdateCustomerPasswordAsync(UserPasswordChangeModel model);

    Task<ValidationResult> ValidateUpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel model);

    Task<ValidationResult> ValidateCustomerLoginAsync(UserLoginModel model);

    Task<ValidationResult> ValidateAdminLoginAsync(UserLoginModel model);
}
