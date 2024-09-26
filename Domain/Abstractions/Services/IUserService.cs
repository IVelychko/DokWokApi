using Domain.Models;
using Domain.Models.User;
using Domain.ResultType;

namespace Domain.Abstractions.Services;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllUsersAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<UserModel>> GetAllCustomersAsync(PageInfo? pageInfo = null);

    Task<UserModel?> GetUserByUserNameAsync(string userName);

    Task<UserModel?> GetUserByIdAsync(string id);

    Task<UserModel?> GetCustomerByIdAsync(string id);

    Task<Result<UserModel>> AddAsync(UserModel model, string password);

    Task<Result<UserModel>> UpdateAsync(UserModel model);

    Task<Result<bool>> UpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword);

    Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(string userId, string newPassword);

    Task<bool?> DeleteAsync(string id);

    Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId);

    Task<Result<AuthorizedUserModel>> LoginAsync(string userName, string password);

    Task<Result<AuthorizedUserModel>> RegisterAsync(UserModel model, string password);

    Task<bool> LogOutAsync(string refreshToken);

    Task<Result<bool>> IsUserNameTakenAsync(string userName);

    Task<Result<bool>> IsEmailTakenAsync(string email);

    Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber);

    Task<UserModel?> GetUserFromTokenAsync(string token);

    Task<Result<AuthorizedUserModel>> RefreshTokenAsync(string securityToken, string refreshToken);
}
