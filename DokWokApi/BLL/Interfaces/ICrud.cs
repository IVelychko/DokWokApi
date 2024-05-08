using DokWokApi.BLL.Models;

namespace DokWokApi.BLL.Interfaces;

public interface ICrud<TModel> where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync();

    Task<TModel?> GetByIdAsync(long id);

    Task<TModel> AddAsync(TModel model);

    Task<TModel> UpdateAsync(TModel model);

    Task DeleteAsync(long id);
}
