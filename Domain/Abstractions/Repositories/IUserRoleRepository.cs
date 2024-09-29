using Domain.Entities;
using Domain.Models;

namespace Domain.Abstractions.Repositories;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetAllAsync(PageInfo? pageInfo = null);

    Task<UserRole?> GetByIdAsync(long id);

    Task<UserRole?> GetByNameAsync(string name);
}
