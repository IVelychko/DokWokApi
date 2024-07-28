using Domain.Entities;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync();

    Task<TEntity?> GetByIdAsync(long id);

    Task<Result<TEntity>> AddAsync(TEntity entity);

    Task<Result<TEntity>> UpdateAsync(TEntity entity);

    Task<bool?> DeleteByIdAsync(long id);
}
