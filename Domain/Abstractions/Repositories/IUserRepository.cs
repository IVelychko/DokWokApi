using Domain.Entities;
using Domain.Helpers;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<User>> GetAllUsersWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<User>> GetAllCustomersAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<User>> GetAllCustomersWithDetailsAsync(PageInfo? pageInfo = null);

    Task<User?> GetUserByUserNameAsync(string userName);

    Task<User?> GetUserByUserNameWithDetailsAsync(string userName);

    Task<User?> GetUserByIdAsync(long id);

    Task<User?> GetUserByIdWithDetailsAsync(long id);

    Task<User?> GetCustomerByIdAsync(long id);

    Task<User?> GetCustomerByIdWithDetailsAsync(long id);

    Task<Result<Unit>> AddAsync(User entity, string password);

    Result<Unit> Update(User entity);

    Task<Result<Unit>> UpdateCustomerPasswordAsync(long userId, string oldPassword, string newPassword);

    Task<Result<Unit>> UpdateCustomerPasswordAsAdminAsync(long userId, string newPassword);

    Task DeleteByIdAsync(long id);

    Task<bool> CheckUserPasswordAsync(long userId, string password);

    bool CheckUserPassword(User user, string password);

    Task<Result<bool>> IsUserNameTakenAsync(string userName);

    Task<Result<bool>> IsEmailTakenAsync(string email);

    Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber);
}
