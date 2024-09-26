using Domain.Entities;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(PageInfo? pageInfo = null);

    Task<IEnumerable<ApplicationUser>> GetAllCustomersAsync(PageInfo? pageInfo = null);

    Task<ApplicationUser?> GetUserByUserNameAsync(string userName);

    Task<ApplicationUser?> GetUserByIdAsync(string id);

    Task<ApplicationUser?> GetCustomerByIdAsync(string id);

    Task<Result<ApplicationUser>> AddAsync(ApplicationUser entity, string password);

    Task<Result<ApplicationUser>> UpdateAsync(ApplicationUser entity);

    Task<Result<bool>> UpdateCustomerPasswordAsync(string userId, string oldPassword, string newPassword);

    Task<Result<bool>> UpdateCustomerPasswordAsAdminAsync(string userId, string newPassword);

    Task<bool?> DeleteAsync(string id);

    Task<Result<IEnumerable<string>>> GetUserRolesAsync(string userId);

    Task<Result<bool>> IsInRoleAsync(ApplicationUser entity, string role);

    Task<Result<bool>> AddToRoleAsync(ApplicationUser entity, string role);

    Task<bool> CheckUserPasswordAsync(ApplicationUser entity, string password);

    Task<Result<bool>> IsUserNameTakenAsync(string userName);

    Task<Result<bool>> IsEmailTakenAsync(string email);

    Task<Result<bool>> IsPhoneNumberTakenAsync(string phoneNumber);
}
