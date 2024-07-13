using DokWokApi.DAL.Entities;
using LanguageExt.Common;

namespace DokWokApi.DAL.Interfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> GetAll();

    Task<TEntity?> GetByIdAsync(long id);

    Task<Result<TEntity>> AddAsync(TEntity entity);

    Task<Result<TEntity>> UpdateAsync(TEntity entity);

    Task<bool?> DeleteByIdAsync(long id);
}
