using Domain.Helpers;
using Domain.Models;

namespace Domain.Abstractions.Services;

public interface ICrud<TModel> where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync(PageInfo? pageInfo = null);

    Task<TModel?> GetByIdAsync(long id);

    Task<Result<TModel>> AddAsync(TModel model);

    Task<Result<TModel>> UpdateAsync(TModel model);

    Task DeleteAsync(long id);
}
