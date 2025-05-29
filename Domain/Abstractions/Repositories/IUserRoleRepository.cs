using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IUserRoleRepository
{
    Task<IList<UserRole>> GetAllAsync();

    Task<UserRole?> GetByIdAsync(long id);

    Task<UserRole?> GetByNameAsync(string name);
    
    Task<bool> UserRoleExistsAsync(long id);
}
