using Domain.Entities;
using Domain.Models;
using Domain.Shared;

namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IList<User>> GetAllUsersBySpecificationAsync(Specification<User> specification);

    Task<IList<User>> GetAllCustomersBySpecificationAsync(Specification<User> specification);

    Task<IList<User>> GetAllUsersAsync(PageInfo? pageInfo = null);

    Task<IList<User>> GetAllUsersWithDetailsAsync(PageInfo? pageInfo = null);

    Task<IList<User>> GetAllCustomersAsync(PageInfo? pageInfo = null);

    Task<IList<User>> GetAllCustomersWithDetailsAsync(PageInfo? pageInfo = null);

    Task<User?> GetUserByUserNameAsync(string userName);

    Task<User?> GetUserByUserNameWithDetailsAsync(string userName);

    Task<User?> GetUserByIdAsync(long id);

    Task<User?> GetUserByIdWithDetailsAsync(long id);

    Task<User?> GetCustomerByIdAsync(long id);

    Task<User?> GetCustomerByIdWithDetailsAsync(long id);

    Task AddAsync(User entity);

    void Update(User entity);

    void Delete(User entity);

    Task<bool> CheckUserPasswordAsync(long userId, string password);

    bool CheckUserPassword(User user, string password);

    Task<bool> IsUserNameUniqueAsync(string userName);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
}
