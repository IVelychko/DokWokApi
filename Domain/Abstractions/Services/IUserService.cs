using Domain.Helpers;
using Domain.Models;
using Domain.Models.User;

namespace Domain.Abstractions.Services;

public interface IUserService
{
    Task<IEnumerable<UserModel>> GetAllUsersAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<UserModel>> GetAllCustomersAsync(PageInfo? pageInfo = null);

    Task<UserModel?> GetUserByUserNameAsync(string userName);

    Task<UserModel?> GetUserByIdAsync(long id);

    Task<UserModel?> GetCustomerByIdAsync(long id);

    Task<Result<UserModel>> AddAsync(UserModel model, string password);

    Task<Result<UserModel>> UpdateAsync(UserModel model);

    Task<Result<Unit>> UpdateCustomerPasswordAsync(long userId, string oldPassword, string newPassword);

    Task<Result<Unit>> UpdateCustomerPasswordAsAdminAsync(long userId, string newPassword);

    Task DeleteAsync(long id);

    Task<Result<AuthorizedUserModel>> LoginAsync(string userName, string password);

    Task<Result<AuthorizedUserModel>> RegisterAsync(UserModel model, string password);

    Task<bool> LogOutAsync(string refreshToken);

    Task<Result<bool>> IsUserNameTakenAsync(string userName);

    Task<Result<bool>> IsEmailTakenAsync(string email);

    Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber);

    Task<UserModel?> GetUserFromTokenAsync(string token);

    Task<Result<AuthorizedUserModel>> RefreshTokenAsync(string securityToken, string refreshToken);
}
