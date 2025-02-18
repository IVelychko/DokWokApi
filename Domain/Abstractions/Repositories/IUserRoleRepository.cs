using Domain.Entities;
using Domain.Models;
using Domain.Shared;

namespace Domain.Abstractions.Repositories;

public interface IUserRoleRepository
{
    Task<IList<UserRole>> GetAllBySpecificationAsync(Specification<UserRole> specification);

    Task<IList<UserRole>> GetAllAsync(PageInfo? pageInfo = null);

    Task<UserRole?> GetByIdAsync(long id);

    Task<UserRole?> GetByNameAsync(string name);
    
    Task<bool> UserRoleExistsAsync(long id);
}
