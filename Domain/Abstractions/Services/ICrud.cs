using Domain.ResultType;

namespace Domain.Abstractions.Services;

public interface ICrud<TModel> where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync();

    Task<IEnumerable<TModel>> GetAllByPageAsync(int pageNumber, int pageSize);

    Task<TModel?> GetByIdAsync(long id);

    Task<Result<TModel>> AddAsync(TModel model);

    Task<Result<TModel>> UpdateAsync(TModel model);

    Task<bool?> DeleteAsync(long id);
}
