using DokWokApi.DAL.Entities;

namespace DokWokApi.DAL.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> GetAll();

    Task<TEntity?> GetByIdAsync(long id);

    Task<TEntity> AddAsync(TEntity entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);

    Task DeleteByIdAsync(long id);
}
