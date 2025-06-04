using Domain.DTOs.Requests.Users;
using Domain.DTOs.Responses;
using Domain.DTOs.Responses.Users;

namespace Domain.Abstractions.Services;

public interface IUserService
{
    Task<IList<UserResponse>> GetAllUsersAsync();

    Task<IList<UserResponse>> GetAllUsersByRoleNameAsync(string roleName);

    Task<IList<UserResponse>> GetAllCustomersAsync();

    Task<UserResponse> GetUserByUserNameAsync(string userName);

    Task<UserResponse> GetUserByIdAsync(long id);

    Task<UserResponse> GetUserByRoleNameAndUserIdAsync(string roleName, long userId);

    Task<UserResponse> GetCustomerByIdAsync(long id);

    Task<UserResponse> AddAsync(AddUserRequest request);

    Task<UserResponse> UpdateAsync(UpdateUserRequest request);

    Task UpdateCustomerPasswordAsync(UpdatePasswordRequest request);

    Task UpdateCustomerPasswordAsAdminAsync(UpdatePasswordAsAdminRequest request);

    Task DeleteAsync(long id);

    Task<IsTakenResponse> IsUserNameTakenAsync(string userName);

    Task<IsTakenResponse> IsEmailTakenAsync(string email);

    Task<IsTakenResponse> IsPhoneNumberTakenAsync(string phoneNumber);
}
