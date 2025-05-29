using Domain.Entities;
using Domain.Specifications.Users;

namespace Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IList<User>> GetAllAsync();

    Task<IList<User>> GetAllBySpecificationAsync(UserSpecification specification);

    Task<IList<User>> GetAllByRoleIdAsync(long roleId);

    Task<User?> GetBySpecificationAsync(UserSpecification specification);

    Task<User?> GetByIdAsync(long id);

    Task<User?> GetByUserNameAsync(string userName);

    Task AddAsync(User entity);

    void Update(User entity);

    void Delete(User entity);

    Task<bool> IsUserNameUniqueAsync(string userName);
    
    Task<bool> IsUserNameUniqueAsync(string userName, long idToExclude);

    Task<bool> IsEmailUniqueAsync(string email);
    
    Task<bool> IsEmailUniqueAsync(string email, long idToExclude);

    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
    
    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, long idToExclude);

    Task<bool> UserExistsAsync(long id);
    
    Task<bool> UserExistsAsync(long userId, long roleIdToExclude);
}
