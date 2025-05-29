using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses.Users;

namespace Domain.Abstractions.Services;

public interface IUserService
{
    Task<IList<UserResponse>> GetAllUsersAsync();

    Task<IList<UserResponse>> GetAllCustomersAsync();

    Task<UserResponse> GetUserByUserNameAsync(string userName);

    Task<UserResponse> GetUserByIdAsync(long id);

    Task<UserResponse> GetCustomerByIdAsync(long id);

    Task<UserResponse> AddAsync(AddUserRequest request);

    Task<UserResponse> UpdateAsync(UpdateUserRequest request);

    Task UpdateCustomerPasswordAsync(long userId, string oldPassword, string newPassword);

    Task UpdateCustomerPasswordAsAdminAsync(long userId, string newPassword);

    Task DeleteAsync(long id);

    Task<bool> IsUserNameUniqueAsync(string userName);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
}
