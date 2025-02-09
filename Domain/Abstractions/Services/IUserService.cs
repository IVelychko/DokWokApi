using Domain.DTOs.Commands.Users;
using Domain.DTOs.Responses.Users;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<UserResponse>> GetAllCustomersAsync(PageInfo? pageInfo = null);

    Task<UserResponse?> GetUserByUserNameAsync(string userName);

    Task<UserResponse?> GetUserByIdAsync(long id);

    Task<UserResponse?> GetCustomerByIdAsync(long id);

    Task<UserResponse> AddAsync(AddUserCommand command);

    Task<UserResponse> UpdateAsync(UpdateUserCommand command);

    Task UpdateCustomerPasswordAsync(long userId, string newPassword);

    Task DeleteAsync(long id);

    Task<AuthorizedUserResponse> LoginAsync(string userName);

    Task<AuthorizedUserResponse> RegisterAsync(RegisterUserCommand command);

    Task<bool> LogOutAsync(string refreshToken);

    Task<bool> IsUserNameUniqueAsync(string userName);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);

    Task<UserResponse?> GetUserFromTokenAsync(string token);

    Task<AuthorizedUserResponse> RefreshTokenAsync(string securityToken, string refreshToken);
}
