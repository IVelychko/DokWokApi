using Domain.Entities;
using Domain.Models;
using Domain.Shared;

namespace Domain.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IList<TEntity>> GetAllAsync(PageInfo? pageInfo = null);

    Task<IList<TEntity>> GetAllBySpecificationAsync(Specification<TEntity> specification);

    Task<TEntity?> GetByIdAsync(long id);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}
