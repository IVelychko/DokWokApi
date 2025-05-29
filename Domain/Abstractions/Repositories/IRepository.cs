using Domain.Entities;

namespace Domain.Abstractions.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IList<TEntity>> GetAllAsync();

    Task<TEntity?> GetByIdAsync(long id);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}
