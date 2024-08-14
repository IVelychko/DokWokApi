using Domain.Entities;
using Domain.ResultType;

namespace Domain.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetAllWithDetailsAsync();

    Task<IEnumerable<Product>> GetAllWithDetailsByPageAsync(int pageNumber, int pageSize);

    Task<IEnumerable<Product>> GetAllByCategoryIdAsync(long categoryId);

    Task<IEnumerable<Product>> GetAllByCategoryIdAndPageAsync(long categoryId, int pageNumber, int pageSize);

    Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAsync(long categoryId);

    Task<IEnumerable<Product>> GetAllWithDetailsByCategoryIdAndPageAsync(long categoryId, int pageNumber, int pageSize);

    Task<Product?> GetByIdWithDetailsAsync(long id);

    Task<Result<bool>> IsNameTakenAsync(string name);
}
