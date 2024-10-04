using Domain.Entities;
using Domain.Helpers;
using Domain.Models;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(PageInfo? pageInfo = null);

    Task<TEntity?> GetByIdAsync(long id);

    Task<Result<Unit>> AddAsync(TEntity entity);

    Result<Unit> Update(TEntity entity);

    Task DeleteByIdAsync(long id);
}
