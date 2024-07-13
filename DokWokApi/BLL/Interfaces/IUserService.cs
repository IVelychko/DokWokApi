using DokWokApi.BLL.Models.User;
using LanguageExt.Common;

namespace DokWokApi.BLL.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllAsync();

    Task<IEnumerable<UserModel>> GetAllCustomersAsync();

    Task<UserModel?> GetByUserNameAsync(string userName);

    Task<UserModel?> GetByIdAsync(string id);

    Task<UserModel?> GetCustomerByIdAsync(string id);

    Task<Result<UserModel>> AddAsync(UserModel model, string password);

    Task<Result<UserModel>> UpdateAsync(UserModel model);

    Task<Result<bool>> UpdateCustomerPasswordAsync(UserPasswordChangeModel model);

    Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(UserPasswordChangeAsAdminModel model);

    Task<bool?> DeleteAsync(string id);

    Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId);

    Task<Result<AuthorizedUserModel>> AuthenticateCustomerLoginAsync(UserLoginModel model);

    Task<Result<AuthorizedUserModel>> AuthenticateAdminLoginAsync(UserLoginModel model);

    Task<Result<AuthorizedUserModel>> AuthenticateRegisterAsync(UserRegisterModel model);

    Task<UserModel?> IsCustomerTokenValidAsync(string token);

    Task<UserModel?> IsAdminTokenValidAsync(string token);

    Task<Result<bool>> IsUserNameTaken(string userName);

    Task<Result<bool>> IsEmailTaken(string email);

    Task<Result<bool>> IsPhoneNumberTaken(string phoneNumber);

    Task<UserModel?> GetUserFromToken(string token);
}
