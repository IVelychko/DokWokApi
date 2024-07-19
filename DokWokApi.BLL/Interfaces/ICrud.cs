using DokWokApi.DAL.ResultType;

namespace DokWokApi.BLL.Interfaces;

public interface ICrud<TModel> where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync();

    Task<TModel?> GetByIdAsync(long id);

    Task<Result<TModel>> AddAsync(TModel model);

    Task<Result<TModel>> UpdateAsync(TModel model);

    Task<bool?> DeleteAsync(long id);
}
